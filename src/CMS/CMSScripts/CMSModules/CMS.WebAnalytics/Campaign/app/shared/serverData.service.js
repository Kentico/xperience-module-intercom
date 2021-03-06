(function (angular, dataFromServer) {
    'use strict';
    
    angular.module('cms.webanalytics/campaign/serverData.service', [])
        .service('serverDataService', serverDataService);

    function serverDataService () {
        this.getDataFromServer = function () {
            return dataFromServer;
        };

        this.getEmailRegexp = function () {
            return dataFromServer.emailRegexp;
        };

        this.getSelectEmailDialogUrl = function () {
            return dataFromServer.selectEmailDialogUrl;
        };

        this.getAssets = function () {
            return dataFromServer.assets;
        }

        this.getUrlAssetTargetRegex = function () {
            return dataFromServer.urlAssetTargetRegex;
        }

        this.getObjective = function () {
            return dataFromServer.objective;
        }
        
    };
}(angular, dataFromServer));
