using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	/// <summary>
	/// This profile is similar to the gravity turn mod. It is a 3-burn to orbit style of launch that can get to orbit with about 2800 dV on stock Kerbin.
	/// If you want to have fun make a rocket that is basically a nose cone, a jumbo-64 a mainsail and some fairly big fins, have the pitch program flip it over aggressively (uncheck the AoA limiter, set the values to like 0.5 / 50 / 40 / 45 / 1) and let it rip.
	/// </summary>
	/// <remarks>
	/// It's not precisely the GT mod algorithm and it does not do any pitch-up during the intermediate burn right now, so it won't handle low TWR upper stages.
	/// </remarks>
	[KRPCClass(Service = "MechJeb")]
	public class AscentGT : AscentBase {
		internal new const string MechJebType = "MuMech.MechJebModuleAscentGT";
		internal static readonly string[] MechJebTypeAliases = { "MuMech.MechJebModuleAscentSettings" };

		// Fields and methods
		private static FieldInfo turnStartAltitudeField;
		private static FieldInfo turnStartVelocityField;
		private static FieldInfo turnStartPitchField;
		private static FieldInfo intermediateAltitudeField;
		private static FieldInfo holdAPTimeField;
		private static bool available;

		// Instance objects
		private object turnStartAltitude;
		private object turnStartVelocity;
		private object turnStartPitch;
		private object intermediateAltitude;
		private object holdAPTime;

		internal static new void InitType(Type type) {
			available = type.FullName == MechJebType;
			turnStartAltitudeField = type.GetOptionalField("turnStartAltitude");
			turnStartVelocityField = type.GetOptionalField("turnStartVelocity");
			turnStartPitchField = type.GetOptionalField("turnStartPitch");
			intermediateAltitudeField = type.GetOptionalField("intermediateAltitude");
			holdAPTimeField = type.GetOptionalField("holdAPTime");
		}

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);
			this.turnStartAltitude = turnStartAltitudeField.GetInstanceValue(instance);
			this.turnStartVelocity = turnStartVelocityField.GetInstanceValue(instance);
			this.turnStartPitch = turnStartPitchField.GetInstanceValue(instance);
			this.intermediateAltitude = intermediateAltitudeField.GetInstanceValue(instance);
			this.holdAPTime = holdAPTimeField.GetInstanceValue(instance);
		}

		/// <summary>
		/// Altitude in km to pitch over and initiate the Gravity Turn (higher values for lower-TWR rockets).
		/// </summary>
		[KRPCProperty]
		public double TurnStartAltitude {
			get => GetEditableDouble(this.turnStartAltitude, nameof(TurnStartAltitude));
			set => SetEditableDouble(this.turnStartAltitude, value, nameof(TurnStartAltitude));
		}

		/// <summary>
		/// Velocity in m/s which triggers pitch over and initiates the Gravity Turn (higher values for lower-TWR rockets).
		/// </summary>
		[KRPCProperty]
		public double TurnStartVelocity {
			get => GetEditableDouble(this.turnStartVelocity, nameof(TurnStartVelocity));
			set => SetEditableDouble(this.turnStartVelocity, value, nameof(TurnStartVelocity));
		}

		/// <summary>
		/// Pitch that the pitch program immediately applies.
		/// </summary>
		[KRPCProperty]
		public double TurnStartPitch {
			get => GetEditableDouble(this.turnStartPitch, nameof(TurnStartPitch));
			set => SetEditableDouble(this.turnStartPitch, value, nameof(TurnStartPitch));
		}

		/// <summary>
		/// Intermediate apoapsis altitude to coast to and then raise the apoapsis up to the eventual final target. May be set to equal the final target in order to skip the intermediate phase.
		/// </summary>
		[KRPCProperty]
		public double IntermediateAltitude {
			get => GetEditableDouble(this.intermediateAltitude, nameof(IntermediateAltitude));
			set => SetEditableDouble(this.intermediateAltitude, value, nameof(IntermediateAltitude));
		}

		/// <summary>
		/// At the intermediate altitude with this much time-to-apoapsis left the engine will start burning prograde to lift the apoapsis.
		/// The engine will throttle down in order to burn closer to the apoapsis.
		/// This is very similar to the lead-time of a maneuver node in concept, but with throttling down in the case where the player has initiated the burn too early (the corollary is that if you see lots of throttling down at the start, you likely need less HoldAP time).
		/// </summary>
		[KRPCProperty]
		public double HoldAPTime {
			get => GetEditableDouble(this.holdAPTime, nameof(HoldAPTime));
			set => SetEditableDouble(this.holdAPTime, value, nameof(HoldAPTime));
		}

		private static double GetEditableDouble(object value, string memberName) {
			if(!available || value == null)
				throw new MJServiceException("This feature is not available in this MechJeb version: AscentGT." + memberName);

			return EditableDouble.Get(value);
		}

		private static void SetEditableDouble(object target, double value, string memberName) {
			if(!available || target == null)
				throw new MJServiceException("This feature is not available in this MechJeb version: AscentGT." + memberName);

			EditableDouble.Set(target, value);
		}
	}
}
