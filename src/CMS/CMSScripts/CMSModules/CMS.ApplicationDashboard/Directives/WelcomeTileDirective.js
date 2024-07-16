cmsdefine(["require", "exports", 'CMS/EventHub'], function(cmsrequire, exports, eventHub) {
    exports.Directive = [
        'cms.resource.welcomeTile', function (__welcomeTile) {
            var tile = {
                restrict: 'A',
                scope: {},
                link: function ($scope) {
                    $scope.model = {};
                    $scope.model.visible = false;

                    const toggleWelcomeTile = (value) => $scope.$emit('toggleWelcomeTile', value);

                    __welcomeTile.get(function (wtData) {
                        $scope.model.header = wtData.Header;
                        $scope.model.description = wtData.Description;
                        $scope.model.browseApplicationsText = wtData.BrowseApplicationsText;
                        $scope.model.openHelpText = wtData.OpenHelpText;

                        // User settings indicate the Welcome tile visibility
                        toggleWelcomeTile(wtData.Visible);
                    });

                    $scope.model.hide = () => {
                        toggleWelcomeTile(false);

                        // Store the hidden welcome tile state in the user settings
                        __welcomeTile.update({ visible: false });
                    }
                },
                templateUrl: 'welcomeTileTemplate.html'
            };

            return tile;
        }
    ];
});
