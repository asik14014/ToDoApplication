using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.Attachment
{
    public class Attachment
    {
        public virtual long Id { get; set; }

        public virtual int FileType { get; set; }

        public virtual string FileName { get; set; }

        public virtual string FileUrl { get; set; }

        public virtual long TaskId { get; set; }

        public virtual DateTime LastUpdate { get; set; }
    }
}
