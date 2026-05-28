using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb.Maneuver {
	/**
	 * <summary>Change surface longitude of apsis</summary>
	 */
	[KRPCClass(Service = "MechJeb")]
	public class OperationLongitude : TimedOperation {
		internal new const string MechJebType = "MuMech.OperationLongitude";

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
		public double NewSurfaceLongitude {
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
