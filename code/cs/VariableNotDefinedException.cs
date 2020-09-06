using System;

namespace frz
{
	public class VariableNotDefinedException : Exception
	{
		public VariableNotDefinedException (string message): base(message)
		{

		}
	}
}

