using System.Collections.Generic;

namespace OrionRehber.Models.ViewModels
{
    public class RehberListVm
    {
        public IEnumerable<Rehber> Kisiler { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
