﻿using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Akka.EventStore.Cqrs.Core.Messages;
using Akka.EventStore.Cqrs.Core.Extensions;

namespace Akka.EventStore.Cqrs.Core
{
    public interface IAggregateRootCreationParameters
    {
        Guid Id { get; }
        IActorRef Projections { get; }
        int SnapshotThreshold { get; }
    }

    public class AggregateRootCreationParameters : IAggregateRootCreationParameters
    {
        public AggregateRootCreationParameters(Guid id, IActorRef projections, int snapshotThreshold = 250)
        {
            Id = id;
            Projections = projections;
            SnapshotThreshold = snapshotThreshold;
        }
        //public AggregateRootCreationParameters(IActorRef projections, int snapshotThreshold = 250)
        //{
        //    Projections = projections;
        //    SnapshotThreshold = snapshotThreshold;
        //}

        public Guid Id { get; private set; }
        public IActorRef Projections { get; private set; }
        public int SnapshotThreshold { get; private set; }
    }

    public abstract class AggregateRootActor : PersistentActor, IEventSink
    {
        private readonly Guid _id;
        private readonly IActorRef _projections;
        private readonly int _snapshotThreshold;
        private readonly ICollection<Exception> _exceptions;
        private readonly ILoggingAdapter _log;
        private long LastSnapshottedVersion { get; set; }

        protected AggregateRootActor(IAggregateRootCreationParameters parameters)
        {
            _id = parameters.Id;
            _projections = parameters.Projections;
            _snapshotThreshold = parameters.SnapshotThreshold;

            _exceptions = new List<Exception>();
            _log = Context.GetLogger();
        }

        public override string PersistenceId
        {
            //get { return string.Format("{0}-agg-{1:n}", GetType().Name, _id).ToLowerInvariant(); }
            get { return _id.ToString(); }
        }

        void IEventSink.Publish(IEvent @event)
        {
            Persist(@event, e =>
            {
                Apply(e);
                _projections.Tell(@event);
            });
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message.CanHandle<RecoveryCompleted>(x =>
            {
                _log.Debug("Recovered state to version {0}", LastSequenceNr);
            }))
                return true;

            var snapshot = message.ReadMessage<SnapshotOffer>();
            if (snapshot.HasValue)
            {
                var offer = snapshot.Value;
                _log.Debug("State loaded from snapshot");
                LastSnapshottedVersion = offer.Metadata.SequenceNr;
                return RecoverState(offer.Snapshot);
            }

            if (message.CanHandle<IEvent>(@event =>
            {
                Apply(@event);
            }))
                return true;

            return false;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message.WasHandled<SaveAggregate>(x => Save()))
                return true;

            if (message.WasHandled<ICommand>(command =>
            {
                try
                {
                    var handled = Handle(command);
                    Sender.Tell(new CommandResponse(handled, _exceptions));
                    return handled;
                }
                catch (Exception e)
                {
                    Sender.Tell(e);
                    return false;
                }
            }))
                return true;

            return false;
        }

        private bool Save()
        {
            if ((LastSequenceNr - LastSnapshottedVersion) >= _snapshotThreshold)
            {
                SaveSnapshot(GetState());
                LastSnapshottedVersion = LastSequenceNr;
            }

            return true;
        }

        protected abstract bool Handle(ICommand command);
        protected abstract bool Apply(IEvent @event);
        protected abstract bool RecoverState(object state);
        protected abstract object GetState();
    }
}