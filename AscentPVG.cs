using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	/// <summary>
	/// The Primer Vector Guidance (RSS/RO) profile.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class AscentPVG : AscentBase {
		internal new const string MechJebType = "MuMech.MechJebModuleAscentPVG";
		internal static readonly string[] MechJebTypeAliases = { "MuMech.MechJebModuleAscentSettings" };

		// Fields and methods
		private static FieldInfo pitchStartVelocityField;
		private static FieldInfo pitchRateField;
		private static FieldInfo desiredApoapsisField;
		private static FieldInfo attachAltFlag;
		private static FieldInfo desiredAttachAltField;
		private static FieldInfo dynamicPressureTriggerField;
		private static FieldInfo stagingTriggerField;
		private static FieldInfo stagingTriggerFlag;
		private static FieldInfo fixedCoast;
		private static FieldInfo fixedCoastLengthField;

		// Instance objects
		private object pitchStartVelocity;
		private object pitchRate;
		private object desiredApoapsis;
		private object desiredAttachAlt;
		private object dynamicPressureTrigger;
		private object stagingTrigger;
		private object fixedCoastLength;

		internal static new void InitType(Type type) {
			pitchStartVelocityField = type.GetOptionalField("PitchStartVelocity");
			pitchRateField = type.GetOptionalField("PitchRate");
			desiredApoapsisField = type.GetOptionalField("DesiredApoapsis");
			attachAltFlag = type.GetOptionalField("AttachAltFlag");
			desiredAttachAltField = type.GetOptionalField("DesiredAttachAlt");
			dynamicPressureTriggerField = type.GetOptionalField("DynamicPressureTrigger");
			stagingTriggerField = type.GetOptionalField("StagingTrigger");
			stagingTriggerFlag = type.GetOptionalField("StagingTriggerFlag");
			fixedCoast = type.GetOptionalField("FixedCoast");
			fixedCoastLengthField = type.GetOptionalField("FixedCoastLength");
		}

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);

			this.pitchStartVelocity = pitchStartVelocityField.GetInstanceValue(instance);
			this.pitchRate = pitchRateField.GetInstanceValue(instance);
			this.desiredApoapsis = desiredApoapsisField.GetInstanceValue(instance);
			this.desiredAttachAlt = desiredAttachAltField.GetInstanceValue(instance);
			this.dynamicPressureTrigger = dynamicPressureTriggerField.GetInstanceValue(instance);
			this.stagingTrigger = stagingTriggerField.GetInstanceValue(instance);
			this.fixedCoastLength = fixedCoastLengthField.GetInstanceValue(instance);
		}

		[KRPCProperty]
		public double PitchStartVelocity {
			get => GetEditableDouble(this.pitchStartVelocity, nameof(PitchStartVelocity));
			set => SetEditableDouble(this.pitchStartVelocity, value, nameof(PitchStartVelocity));
		}

		[KRPCProperty]
		public double PitchRate {
			get => GetEditableDouble(this.pitchRate, nameof(PitchRate));
			set => SetEditableDouble(this.pitchRate, value, nameof(PitchRate));
		}

		/// <summary>
		/// The target apoapsis in meters.
		/// </summary>
		[KRPCProperty]
		public double DesiredApoapsis {
			get => GetEditableDouble(this.desiredApoapsis, nameof(DesiredApoapsis));
			set => SetEditableDouble(this.desiredApoapsis, value, nameof(DesiredApoapsis));
		}

		[KRPCProperty]
		public bool AttachAltFlag {
			get => GetBoolean(attachAltFlag, nameof(AttachAltFlag));
			set => SetBoolean(attachAltFlag, value, nameof(AttachAltFlag));
		}

		[KRPCProperty]
		public double DesiredAttachAlt {
			get => GetEditableDouble(this.desiredAttachAlt, nameof(DesiredAttachAlt));
			set => SetEditableDouble(this.desiredAttachAlt, value, nameof(DesiredAttachAlt));
		}

		[KRPCProperty]
		public double DynamicPressureTrigger {
			get => GetEditableDouble(this.dynamicPressureTrigger, nameof(DynamicPressureTrigger));
			set => SetEditableDouble(this.dynamicPressureTrigger, value, nameof(DynamicPressureTrigger));
		}

		[KRPCProperty]
		public int StagingTrigger {
			get => GetEditableInt(this.stagingTrigger, nameof(StagingTrigger));
			set => SetEditableInt(this.stagingTrigger, value, nameof(StagingTrigger));
		}

		[KRPCProperty]
		public bool StagingTriggerFlag {
			get => GetBoolean(stagingTriggerFlag, nameof(StagingTriggerFlag));
			set => SetBoolean(stagingTriggerFlag, value, nameof(StagingTriggerFlag));
		}

		[KRPCProperty]
		public bool FixedCoast {
			get => GetBoolean(fixedCoast, nameof(FixedCoast));
			set => SetBoolean(fixedCoast, value, nameof(FixedCoast));
		}

		[KRPCProperty]
		public double FixedCoastLength {
			get => GetEditableDouble(this.fixedCoastLength, nameof(FixedCoastLength));
			set => SetEditableDouble(this.fixedCoastLength, value, nameof(FixedCoastLength));
		}

		private static double GetEditableDouble(object value, string memberName) {
			if(value == null)
				throw new MJServiceException("This feature is not available in this MechJeb version: AscentPVG." + memberName);

			return EditableDouble.Get(value);
		}

		private static void SetEditableDouble(object target, double value, string memberName) {
			if(target == null)
				throw new MJServiceException("This feature is not available in this MechJeb version: AscentPVG." + memberName);

			EditableDouble.Set(target, value);
		}

		private static int GetEditableInt(object value, string memberName) {
			if(value == null)
				throw new MJServiceException("This feature is not available in this MechJeb version: AscentPVG." + memberName);

			return EditableInt.Get(value);
		}

		private static void SetEditableInt(object target, int value, string memberName) {
			if(target == null)
				throw new MJServiceException("This feature is not available in this MechJeb version: AscentPVG." + memberName);

			EditableInt.Set(target, value);
		}

		private bool GetBoolean(FieldInfo field, string memberName) {
			if(field == null)
				throw new MJServiceException("This feature is not available in this MechJeb version: AscentPVG." + memberName);

			return (bool)field.GetValue(this.instance);
		}

		private void SetBoolean(FieldInfo field, bool value, string memberName) {
			if(field == null)
				throw new MJServiceException("This feature is not available in this MechJeb version: AscentPVG." + memberName);

			field.SetValue(this.instance, value);
		}
	}
}
