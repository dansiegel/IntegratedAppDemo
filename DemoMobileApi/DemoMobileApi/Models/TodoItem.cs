using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoMobileApi.Models
{
    public class TodoItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public bool Done { get; set; }

        public string UserName { get; set; }
    }
}
