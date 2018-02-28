﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {

	[AutomationClass]
	[DebugClass]
	[Deferr]
	[TestRunnerFlag(TestFlag.DisregardSetUpClassGlobal, TestFlag.DisregardTearDownClassGlobal)]
	public class RunnerFlagTests : MonoBehaviour {

		public static bool TryCompleteAfterFailToken { get; set; }
		public static bool SetUpClassGlobalRun { get; set; }
		public static bool SetUpGlobalRun { get; set; }
		public static bool SetUpClassRun { get; set; }
		public static bool SetUpRun { get; set; }
		public static bool TearDownClassGlobalRun { get; set; }
		public static bool TearDownGlobalRun { get; set; }
		public static bool TearDownClassRun { get; set; }
		public static bool TearDownRun { get; set; }

		[SetUpClass]
		public IEnumerator SetUpClass() {

			TryCompleteAfterFailToken = SetUpGlobalRun = SetUpClassGlobalRun = TearDownGlobalRun = TearDownClassGlobalRun = TearDownClassRun = false; //Reset
			SetUpClassRun = true;
			yield return null;

		}

		[SetUp]
		public IEnumerator SetUp() {

			TearDownRun = false; //Reset
			SetUpRun = true; //Set
			yield return null;

		}

		[Automation("Debug/Runner Flags")]
		[Automation("Trilleon/Validation")]
		[TestRunnerFlag(TestFlag.DisregardSetUpClassGlobal)]
		[Validate(Expect.Success)]
		public IEnumerator Flags_DisregardSetUpClassGlobal_SetUpGlobal_SetUpClass_SetUp() {

			Q.assert.IsTrue(SetUpClassRun, "This test should have the SetUpClass support method run before launch.");
			Q.assert.IsTrue(SetUpRun, "This test should have the SetUp support method run before launch.");
			Q.assert.IsTrue(SetUpGlobalRun, "This class should have the SetUpGlobal support method run before launch.");
			Q.assert.IsTrue(!SetUpClassGlobalRun, "This class should NOT have the SetUpGlobalClass support method run before launch.");
			yield return null;

		}

		[Automation("Debug/Runner Flags")]
		[Automation("Trilleon/Validation")]
		[TestRunnerFlag(TestFlag.DisregardSetUpGlobal)]
		[Validate(Expect.Success)]
		public IEnumerator Flags_DisregardSetUpGlobal() {

			Q.assert.IsTrue(!SetUpGlobalRun, "This test should NOT have the SetUpGlobal support method run before launch.");
			yield return null;

		}

		[Automation("Debug/Runner Flags")]
		[Automation("Trilleon/Validation")]
		[TestRunnerFlag(TestFlag.DisregardSetUpTest)]
		[Validate(Expect.Success)]
		public IEnumerator Flags_DisregardSetUpTest_TearDownGlobal_TearDown() {

			Q.assert.IsTrue(!SetUpRun, "This test should NOT have the SetUp support method run before test launch.");
			Q.assert.IsTrue(TearDownRun, "The previous test should have the TearDown support method run after completion.");
			Q.assert.IsTrue(TearDownGlobalRun, "The previous test should have the TearDownGlobal support method run after completion.");
			yield return null;

		}

		[Automation("Debug/Runner Flags")]
		[Automation("Trilleon/Validation")]
		[TestRunnerFlag(TestFlag.TryCompleteAfterFail, TestFlag.DisregardTearDownTest, TestFlag.DisregardTearDownGlobal)]
		[Validate(Expect.Failure)]
		public IEnumerator Flags_TryCompleteAfterFail() {

			Q.assert.IsTrue(false, "Fail test to see that we continue execution, but do not record assertions.");
			yield return StartCoroutine(Q.driver.WaitRealTime(1f));
			Q.assert.IsTrue(true, "This passed assertion should not appear in the assertions list VERIFICATION_CODE[X7821!].");
			Q.assert.IsTrue(false, "This failed assertion should not appear in the assertions list VERIFICATION_CODE[X7822!].");
			yield return StartCoroutine(Q.driver.WaitRealTime(1f));
			Q.assert.IsTrue(AutomationMaster.CurrentTestContext.Assertions.Find(x => x.Contains("VERIFICATION_CODE[X7821!]")) == null, "Passed assertions should not be logged after a failure occurs in a \"TryCompleteAfterFail\" test method.");
			Q.assert.IsTrue(AutomationMaster.CurrentTestContext.Assertions.Find(x => x.Contains("VERIFICATION_CODE[X7822!]")) == null, "Failed assertions should not be logged after a failure occurs in a \"TryCompleteAfterFail\" test method.");
			TryCompleteAfterFailToken = true;
			yield return null;

		}

		[Automation("Debug/Runner Flags")]
		[Automation("Trilleon/Validation")]
		[Validate(Expect.Success)]
		[DependencyWeb("Flags_TryCompleteAfterFail")]
		public IEnumerator Flags_DisregardTearDownTest_DisregardTearDownGlobal() {

			Q.assert.IsTrue(!TearDownRun, "The previous test should not have the TearDown support method run after completion.");
			Q.assert.IsTrue(!TearDownGlobalRun, "The previous test should not have the TearDownGlobal support method run after completion.");
			yield return null;

		}

		[Automation("Debug/Runner Flags")]
		[Automation("Trilleon/Validation")]
		[TestRunnerFlag(TestFlag.OnlyLaunchWhenExplicitlyCalled)]
		[Validate(Expect.Success)]
		public IEnumerator Flags_OnlyLaunchWhenExplicitlyCalled() {

			Q.assert.IsTrue(true, "This test should not be included in the tests run by Validation.");
			yield return null;

		}

		[Automation("Debug/Runner Flags")]
		[Automation("Trilleon/Validation")]
		[TestRunnerFlag(TestFlag.DependencyNoSkip)]
		[Validate(Expect.Success)]
		[DependencyWeb("DependencyMasterTest_05")] //Explicit failure.
		public IEnumerator Flags_DependencyNoSkip() {

			Q.assert.IsTrue(true, "This test should NOT be skipped, despite the dependency failing. This is due to the DependencyNoSkip TestFlag.");
			Q.assert.IsTrue(true, "Why would you use this? Wouldn't you just not mark any tests as a dependency? The current test will not be run until the dependency has run, as a normal dependency would, but the logic that skips dependent tests simply is ignored.");
			yield return null;

		}

		//TearDownClassGlobal logic is checked in Validation method after all tests have run.

		[TearDown]
		public IEnumerator TearDown() {

			SetUpRun = false; //Reset
			TearDownRun = true; //Set
			yield return null;

		}

		[TearDownClass]
		public IEnumerator TearDownClass() {

			SetUpClassRun = SetUpClassGlobalRun = SetUpGlobalRun = false; //Reset
			TearDownClassRun = true;
			yield return null;

		}

	}

}