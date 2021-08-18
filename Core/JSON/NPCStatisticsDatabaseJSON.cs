using AARPG.Core.Mechanics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace AARPG.Core.JSON{
	[DataContract]
	public class NPCStatisticsDatabaseJSON{
		[DataMember(Name = "entries")]
		[DefaultValue(null)]
		public IList<NPCStatisticsDatabaseEntryJSON> Database{ get; set; }
	}
}
