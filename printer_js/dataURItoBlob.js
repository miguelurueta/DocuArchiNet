 // 转换为 blob 直接传输 暂时无用
            function dataURItoBlob(dataURI) {

                // convert base64 to raw binary data held in a string
                // doesn't handle URLEncoded DataURIs
                var byteString = atob(dataURI.split(',')[1]);

                // separate out the mime component
                var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

                // write the bytes of the string to an ArrayBuffer
                var ab = new ArrayBuffer(byteString.length);
                var ia = new Uint8Array(ab);
                for (var i = 0; i < byteString.length; i++) {
                    ia[i] = byteString.charCodeAt(i);
                }


                try {
                    // 新版本浏览器
                    return new $window.Blob([ia], {type: mimeString});
                } catch (e) {

                    // TypeError old chrome and FF
                    // Android 中该方式无效
                    $window.BlobBuilder = $window.BlobBuilder ||
                        $window.WebKitBlobBuilder ||
                        $window.MozBlobBuilder ||
                        $window.MSBlobBuilder;

                    if (e.name == 'TypeError' && $window.BlobBuilder) {

                        var bb = new $window.BlobBuilder();
                        bb.append(ab);
                        return bb.getBlob(mimeString);

                    } else {
                        return null;
                    }
                }
            }