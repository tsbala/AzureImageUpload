var app = angular.module('app', ['angularFileUpload']);

app.controller('azureImageUpload', ['$scope', '$upload', function($scope, $upload){
    $scope.onFileSelect = function($files) {
        //$files: an array of files selected, each file has name, size, and type.
        for (var i = 0; i < $files.length; i++) {
            var file = $files[i];
            $scope.upload = $upload.upload({
                url: 'http://jgimages.azurewebsites.net/api/image/', //upload.php script, node.js route, or servlet url
                method: 'POST',
                headers: {'content-type': 'multipart/form-data'},
                // withCredentials: true,
                //data: {myObj: $scope.myModelObj},
                file: file
            }).progress(function(evt) {
                    console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
            }).success(function(data, status, headers, config) {
                    // file is uploaded successfully
                    $scope.imageUrl = "http://jgimages.azurewebsites.net/azure/" + data.Identifier + "/" + data.Name;
                    $scope.imageUrl160x160 = "http://jgimages.azurewebsites.net/azure/" + data.Identifier + "/160/160/" + data.Name;
                    $scope.imageUrl400x400 = "http://jgimages.azurewebsites.net/azure/" + data.Identifier + "/400/400/" + data.Name;
                });
        }
    }
}]);