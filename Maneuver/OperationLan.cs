using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb.Maneuver {
	/**
	 * <summary>Change longitude of ascending node</summary>
	 */
	[KRPCClass(Service = "MechJeb")]
	public class OperationLan : TimedOperation {
		internal new const string MechJebType = "MuMech.OperationLan";

		// Fields and methods
		private static FieldInfo newLANField;
		private static FieldInfo timeSelector;

		// Instance objects
		private object newLAN;

		internal static new void InitType(Type type) {
			newLANField = type.GetOptionalField("newLAN");
			timeSelector = GetTimeSelectorField(type);
		}

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);

			this.newLAN = newLANField.GetInstanceValue(instance) ?? MechJeb.TargetController.TargetLongitude;
			this.InitTimeSelector(timeSelector);
		}

		[KRPCProperty]
		public double NewLAN {
			get => newLANField != null ? EditableDouble.Get(this.newLAN) : EditableAngle.Get(this.newLAN);
			set {
				if(newLANField != null)
					EditableDouble.Set(this.newLAN, value);
				else
					EditableAngle.Set(this.newLAN, value);
			}
		}
	}
}
