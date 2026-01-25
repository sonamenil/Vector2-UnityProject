using System;
using System.Collections.Generic;
using System.IO;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;

namespace Assets.src.GUI.Dialogs
{
    public class NewsDialogContent : DialogContent
    {
        [SerializeField]
        private ResolutionImage _Image;

        private Action<BaseDialog> _ImageAction;

        public void Init(string imagePath, Action<BaseDialog> imageAction, List<DialogButtonData> buttonsInfo)
        {
            LoadImage(imagePath);
            _ImageAction = imageAction;
            Init(buttonsInfo);
        }

        private void LoadImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }


            Texture2D textureFromExternal = ResourceManager.GetTexture(imagePath);
            Sprite sprite = Sprite.Create(textureFromExternal, new Rect(0f, 0f, textureFromExternal.width, textureFromExternal.height), _Image.rectTransform.pivot);
            _Image.sprite = sprite;
            if (_Image.mainTexture != null)
            {
                _Image.gameObject.SetActive(true);
            }


        }

        public void OnImageTapped()
        {
            if (_ImageAction != null)
            {
                _ImageAction(base.Parent);
            }
        }
    }
}
