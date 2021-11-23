using System;

using CMS.Activities;
using CMS.Core;
using CMS.WebAnalytics.Internal;

namespace Kentico.Xperience.Intercom
{
    /// <summary>
    /// Activity initializer for custom activities logged through the Intercom webhook.
    /// </summary>
    internal class IntercomActivityInitializer : CustomActivityInitializerBase
    {
        private readonly string activityType;
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
        /// <param name="activityComment">Optional activity comment.</param>
        /// <exception cref="ArgumentException">When <paramref name="activityUrl"/> is null or empty.</exception>
        public IntercomActivityInitializer(string activityType, string activityValue, string activityUrl, string activityComment = null)
            : this(Service.Resolve<IActivityUrlHashService>())
        {
            this.activityType = activityType;
            this.activityValue = activityValue;
            this.activityUrl = activityUrl;
            this.activityComment = activityComment;
        }


        private IntercomActivityInitializer(IActivityUrlHashService activityUrlHashService)
        {
            this.activityUrlHashService = activityUrlHashService ?? throw new ArgumentNullException(nameof(activityUrlHashService));
            titleBuilder = new ActivityTitleBuilder();
        }


        public override string ActivityType => activityType;


        /// <summary>
        /// Initializes <see cref="IActivityInfo"/> properties.
        /// </summary>
        /// <param name="activity">Activity info</param>
        public override void Initialize(IActivityInfo activity)
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
    }
}
