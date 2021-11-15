using System;

using CMS.Activities;
using CMS.Core;
using CMS.WebAnalytics.Internal;

namespace Kentico.Xperience.Intercom.Admin
{
    /// <summary>
    /// Activity initializer for activities logged through the Intercom webhook.
    /// </summary>
    internal class IntercomActivityInitializer : IActivityInitializer
    {
        private readonly string activityValue;
        private readonly string activityUrl;
        private readonly string activityComment;

        private readonly ActivityTitleBuilder titleBuilder;
        private readonly IActivityUrlHashService activityUrlHashService;


        /// <summary>
        /// Constructor for intercom activity initializer of the provided type.
        /// </summary>
        /// <param name="activityValue">Activity value.</param>
        /// <param name="activityUrl">Url where activity occurred.</param>
        /// <param name="referrerUrl">Url referrer.</param>
        /// <exception cref="ArgumentException">When <paramref name="activityUrl"/> is null or empty.</exception>
        public IntercomActivityInitializer(string activityType, string activityValue, string activityUrl, string conversationHistory)
            : this(Service.Resolve<IActivityUrlHashService>())
        {
            ActivityType = activityType;

            this.activityValue = activityValue;
            this.activityUrl = activityUrl;
            activityComment = conversationHistory?.Replace(Environment.NewLine, "<br/>");
        }


        private IntercomActivityInitializer(IActivityUrlHashService activityUrlHashService)
        {
            this.activityUrlHashService = activityUrlHashService ?? throw new ArgumentNullException(nameof(activityUrlHashService));
            titleBuilder = new ActivityTitleBuilder();
        }


        /// <summary>
        /// Initializes <see cref="IActivityInfo"/> properties.
        /// </summary>
        /// <param name="activity">Activity info</param>
        public void Initialize(IActivityInfo activity)
        {
            activity.ActivityValue = activityValue;
            activity.ActivityTitle = titleBuilder.CreateTitle(ActivityType, activityValue);
            activity.ActivityComment = activityComment;

            if (!String.IsNullOrEmpty(activityUrl))
            {
                activity.ActivityURL = activityUrl;
                activity.ActivityURLHash = activityUrlHashService.GetActivityUrlHash(activityUrl);
            }
        }


        /// <summary>
        /// Activity type
        /// </summary>
        public string ActivityType
        {
            get;
            private set;
        }


        /// <summary>
        /// Activity settings key name, used to check whether activity logging is enabled.
        /// </summary>
        public string SettingsKeyName
        {
            get
            {
                return String.Empty;
            }
        }
    }
}
