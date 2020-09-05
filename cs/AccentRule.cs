using System;
using System.Text.RegularExpressions;
using ExpressionEvaluator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace frz
{
	/// <summary>
	/// Accent rule
	/// %X "Y Z {Y: ^.*:.*$}
	/// 	Pattern: X Y Z
	/// 	primStress: 1
	/// 	secStress: 0
	/// 	conditions: Y: ^.*:.*$
	/// </summary>
	public class AccentRule
	{
		private List<String> pattern;
		private int primStress;
		private int secStress;
		private List<Condition> conditions = new List<Condition>();

		public AccentRule(string pattern, string conditions)
		{
			this.primStress = Array.FindIndex(pattern.Split(' '), s => s.StartsWith("\"")) + 1;
			this.secStress = Array.FindIndex(pattern.Split(' '), s => s.StartsWith("%")) + 1;
			this.pattern = pattern.Replace("%", "").Replace("\"", "").Split(' ').ToList();
			this.pattern.Insert(0, "#");
			this.pattern.Add("#");
			if (conditions == "")
			{
				this.conditions = null;
			}
			else
			{
				/*  {Y: ^.*:.*$; ...}
				*/
				conditions = conditions.Trim();
				string[] cs = conditions.Split(';');

				foreach (string condition in cs)
				{

					Match condRegex = Regex.Match(condition, @"^(.*?):(.*)$");
					if (condRegex.Success)
					{
						string var = condRegex.Groups[1].Value.Trim();
						string s = condRegex.Groups[2].Value.Trim();
						Condition c = new Condition(var, s);
						this.conditions.Add(c);

					}
					else
					{
						throw new SymbolNotFoundException("Syntax für Bedingungen: \"<Variable>:<Bedingung>\" (gefunden: " + condition + ")");
					}
				}
			}

		}

		public List<String> Pattern
		{
			get { return this.pattern; }
			set { this.pattern = value; }
		}

		public int PrimStress
		{
			get { return this.primStress; }
			set { this.primStress = value; }
		}

		public int SecStress
		{
			get { return this.secStress; }
			set { this.secStress = value; }
		}

		public List<Condition> Conditions
		{
			get
			{
				return this.conditions;
			}
			set { this.conditions = value; }
		}
	}
}

