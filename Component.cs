using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace AnimpafGE.ECS
{
	class Component
	{
		public Entity Entity { get; set; }
		string Name { get; set; }
		bool Enabled { get; set; } = true;

		public virtual void Process()
		{
			Trace.WriteLine("Base process method");
			if(Enabled)
				this.Process();
		}
	}
}
