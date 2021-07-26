using System;
using UnityEngine;

namespace GWLPXL.ARPG._Scripts.Attributes.com
{
	// currently percent is float. 0.1f = 10%. maybe change it
	public enum StatModType
	{
		Flat = 100,
		PercentAdd = 200,
		PercentMult = 300,
	}
	// maybe do as class
	[Serializable]
	public struct AttributeModifier
	{
		[SerializeField] private float _value;
		[SerializeField] private StatModType _type;
		[SerializeField] private int _order;
		private object _source;
		public float Value => _value;
		public StatModType Type => _type;
		public int Order => _order;
		public object Source => _source;

		public AttributeModifier(float value, StatModType type, int order, object source)
		{
			_value = value;
			_type = type;
			_order = order;
			_source = source;
		}

		public AttributeModifier(float value, StatModType type) : this(value, type, (int)type, null) { }

		public AttributeModifier(float value, StatModType type, int order) : this(value, type, order, null) { }

		public AttributeModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
	}
}
