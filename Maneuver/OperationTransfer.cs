using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb.Maneuver {
	/// <summary>
	/// Bi-impulsive (Hohmann) transfer to target.
	/// 
	/// This option is used to plan transfer to target in single sphere of influence. It is suitable for rendezvous with other vessels or moons.
	/// Contrary to the name, the transfer is often uni-impulsive. You can select when you want the manevuer to happen or select optimum time.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class OperationTransfer : TimedOperation {
		internal new const string MechJebType = "MuMech.OperationGeneric";

		// Fields and methods
		private static FieldInfo interceptOnly;
		private static FieldInfo planCapture;
		private static FieldInfo periodOffsetField;
		private static FieldInfo lagTimeField;
		private static FieldInfo simpleTransfer;
		private static FieldInfo coplanar;
		private static FieldInfo timeSelector;

		// Instance objects
		private object periodOffset;

		internal static new void InitType(Type type) {
			interceptOnly = type.GetOptionalField("intercept_only");
			planCapture = type.GetOptionalField("PlanCapture");
			periodOffsetField = type.GetOptionalField("periodOffset");
			lagTimeField = type.GetOptionalField("LagTime");
			simpleTransfer = type.GetOptionalField("simpleTransfer");
			coplanar = type.GetOptionalField("Coplanar");
			timeSelector = GetTimeSelectorField(type);
		}

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);

			this.periodOffset = (periodOffsetField ?? lagTimeField).GetInstanceValue(instance);
			this.InitTimeSelector(timeSelector);
		}

		/// <summary>
		/// Intercept only, no capture burn (impact/flyby)
		/// </summary>
		[KRPCProperty]
		public bool InterceptOnly {
			get {
				if(interceptOnly != null)
					return (bool)interceptOnly.GetValue(this.instance);
				return planCapture != null && !(bool)planCapture.GetValue(this.instance);
			}
			set {
				if(interceptOnly != null)
					interceptOnly.SetValue(this.instance, value);
				else if(planCapture != null)
					planCapture.SetValue(this.instance, !value);
			}
		}

		/// <summary>
		/// Fractional target period offset
		/// </summary>
		[KRPCProperty]
		public double PeriodOffset {
			get => EditableDouble.Get(this.periodOffset);
			set => EditableDouble.Set(this.periodOffset, value);
		}

		/// <summary>
		/// Simple coplanar Hohmann transfer.
		/// Set it to true if you are used to the old version of transfer maneuver.
		/// </summary>
		/// <remarks>If set to true, TimeSelector property is ignored.</remarks>
		[KRPCProperty]
		public bool SimpleTransfer {
			get {
				if(simpleTransfer != null)
					return (bool)simpleTransfer.GetValue(this.instance);
				return coplanar != null && (bool)coplanar.GetValue(this.instance);
			}
			set {
				if(simpleTransfer != null)
					simpleTransfer.SetValue(this.instance, value);
				else if(coplanar != null)
					coplanar.SetValue(this.instance, value);
			}
		}
	}
}
