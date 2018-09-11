using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.Attachment
{
    public class AttachmentType
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime LastUpdate { get; set; }
    }
}
