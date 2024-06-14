/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using UnityEngine;
using System.IO;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class CameraFeedSubscriber : UnitySubscriber<MessageTypes.Sensor.CompressedImage>
    {
        public MeshRenderer meshRenderer;

        private Texture2D texture2D;
        private byte[] imageData;
        private bool isMessageReceived;
        public bool save_bool;

        protected override void Start()
        {
            base.Start();
            texture2D = new Texture2D(1, 1);
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
        }

        protected override void ReceiveMessage(MessageTypes.Sensor.CompressedImage compressedImage)
        {
            imageData = compressedImage.data;
            isMessageReceived = true;
        }

        private void ProcessMessage()
        {
            texture2D.LoadImage(imageData);
            texture2D.Apply();
            meshRenderer.material.SetTexture("_MainTex", texture2D);
            isMessageReceived = false;

            if (save_bool == true){
                byte[] bytes = texture2D.EncodeToPNG();
                string filename = "Screenshot" + DateTime.Now.ToString("_MM-dd-yyyy_hh-mm-ss-f") + ".jpg";
                File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);

                //var cameraRollFolder = Windows.Storage.KnownFolders.CameraRoll.Path;
                //File.Move(_filePath, Path.Comvine(cameraRollFolder, _filename));

                #if !UNITY_EDITOR && UNITY_WINRT_10_0
                        var cameraRollFolder = Windows.Storage.KnownFolders.CameraRoll.Path;            
                        File.Move(_filePath, Path.Combine(cameraRollFolder, _filename));
                #endif


                Debug.Log("Saved Image");
                save_bool = false;
            }
        }

        public void img_save()
        {
            save_bool = !save_bool;
        }
    }
}

