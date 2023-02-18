using System;

namespace Infrastructure.Security.Models
{
    [Serializable]
    public class Session
    {
        #region Constants

        /// <summary>
        /// Время до истечения сессии, после которого поле Status устанавливается в значение SessionStatus.ExpiredSoon.
        /// </summary>
        private static readonly TimeSpan ExpiredSoonTimeSpan = new TimeSpan(1, 20, 0);

        private static readonly TimeSpan OffsetTimeSpan = new TimeSpan(0, 1, 0);

        #endregion Constants

        #region Properties

        /// <summary>
        /// Идентификатор сессии.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Дата и время начала действия сессии.
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>
        /// Дата и время окончания действия сессии.
        /// </summary>
        public DateTime EndDateTime { get; private set; }

        /// <summary>
        /// Состояние сессии.
        /// </summary>
        public SessionStatus Status
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                SessionStatus status;

                if (now < StartDateTime.Subtract(OffsetTimeSpan) || now > EndDateTime.Add(OffsetTimeSpan))
                {
                    status = SessionStatus.Expired;
                }
                else
                {
                    if (now < EndDateTime.Subtract(ExpiredSoonTimeSpan))
                    {
                        status = SessionStatus.Active;
                    }
                    else
                    {
                        status = SessionStatus.ExpiredSoon;
                    }
                }

                return status;
            }
        }

        #endregion Properties

        #region Constructors

        public Session(Guid id, DateTime startDateTime, DateTime endDateTime)
        {
            Id = id;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }

        #endregion Constructors
    }
}
