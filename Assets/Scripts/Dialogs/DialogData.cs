using UnityEngine;
using UnityEngine.Events;

namespace TopDonShooter.Dialogs
{
    public class DialogData {
        public readonly string Title;
        public readonly string Message;
        public readonly Sprite imageSprite;
        public readonly string ButtonTitle;
        public readonly UnityAction ButtonAction;
        public readonly bool ShowCloseButton;

        public DialogData(
            string title, 
            string message, 
            Sprite imageSprite, 
            string buttonTitle, 
            UnityAction buttonAction,
            bool showCloseButton = true)
        {
            Title = title;
            Message = message;
            this.imageSprite = imageSprite;
            ButtonTitle = buttonTitle;
            ButtonAction = buttonAction;
            ShowCloseButton = showCloseButton;
        }
    }
}