var app = angular.module('app', ['angularFileUpload']);

app.controller('azureImageUpload', ['$scope', '$upload', '$timeout', function($scope, $upload, $timeout){
    $scope.onFileSelect = function($files) {
        //$files: an array of files selected, each file has name, size, and type.
        for (var i = 0; i < $files.length; i++) {
            var file = $files[i];
            $scope.upload = $upload.upload({
                url: 'http://jgimages.azurewebsites.net/api/image/', //upload.php script, node.js route, or servlet url
                method: 'POST',
                headers: {'Content-Type': 'multipart/form-data',
                          'Content-Disposition': 'form-data;name="fieldNameHere"; filename="' + $scope.myModelObj + '"',
                          'Content-Type': 'image/jpeg'},
                // withCredentials: true,
                file: file
            }).progress(function(evt) {
                    console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
            }).success(function(data, status, headers, config) {
                    // file is uploaded successfully
                    $timeout(function(){
                        var image = data[0];
                        var blobUrl = "http://jgimages.azurewebsites.net/azure/images/";
                        $scope.imageUrl = blobUrl + image.Identifier + "/" + image.Name;
                        $scope.imageUrl160x160 = blobUrl + image.Identifier + "/160/160/" + image.Name;
                        $scope.imageUrl400x400 = blobUrl + image.Identifier + "/400/400/" + image.Name;
                    }, 3000);
                });
        }
    }
}]);