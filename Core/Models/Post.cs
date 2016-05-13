using System;
using System.Collections.Generic;
using Windows.Data.Html;

namespace Core.Models {
    public class Post {
        public Post(long number,
                    long parentNumber,
                    string subject, string name,
                    string trip, string email,
                    string text, bool isOp,
                    bool isBanned, bool isClosed,
                    bool isSticky,
                    IList<Attachment> attachments,
                    DateTime date) {
            Number = number;
            ParentNumber = parentNumber;
            Subject = subject;
            Name = Utils.Html.RemoveHtml(name).Replace("&nbsp;"," ");
            Trip = trip;
            Email = email;
            Text = text;
            IsOp = isOp;
            IsBanned = isBanned;
            IsClosed = isClosed;
            IsSticky = isSticky;
            Attachments = attachments;
            Date = date;
        }

        public long Number { get; }
        public long ParentNumber { get; }
        public string Subject { get; }
        public string Name { get; }
        public string Trip { get; }
        public string Email { get; }
        public string Text { get; }
        public bool IsOp { get; }
        public bool IsBanned { get; }
        public bool IsClosed { get; }
        public bool IsSticky { get; }
        public IList<Attachment> Attachments { get; }
        public DateTime Date { get; }

        protected bool Equals(Post other) {
            return Number == other.Number && ParentNumber == other.ParentNumber;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Post) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Number.GetHashCode()*397) ^ ParentNumber.GetHashCode();
            }
        }
    }
}