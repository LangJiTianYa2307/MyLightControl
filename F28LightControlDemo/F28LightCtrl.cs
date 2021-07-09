using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace F28LightControlDemo
{
    public class F28LightCtrl
    {
        public const short F28_OK = 0;
        public const short F28_FAIL = -1;

        // 1.404
        public const short F28_CONNECTED = 1;
        public const short F28_OFFLINE = 0;

#if _64BITS
     private const string strDllName = "F28LightControl_ETH64.dll";
#else
        private const string strDllName = "F28LightControl_ETH.dll";
#endif

        private const ushort usSizeVersion = 20;
        private const ushort usSizeSerialNumber = 20;

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct F28_DATE
        {
            public ushort usYear;
            public ushort usMonth;
            public ushort usDay;
            public ushort usHour;
            public ushort usMinute;
            public ushort usSecond;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct F28_PARAMETERS
        {
            public ushort usTypeTest;		// STANDARD or LARGE LEAK (Sealed) or LARGE LEAK TEST (sealed + test)
            public ushort usTpsFillVol;
            public ushort usTpsTransfert;
            public ushort usTpsFill;
            public ushort usTpsStab;
            public ushort usTpsTest;
            public ushort usTpsDump;
            public ushort usPress1Unit;	    // See F28_PRESS_UNITS
            public float fPress1Min;
            public float fPress1Max;
            public float fSetFillP1;		// consigne mode auto-fill
            public float fRatioMax;	        // LARGE LEAK mode only
            public float fRatioMin;	        // LARGE LEAK mode only
            public ushort usFillMode;		// see FILL_MODE
            public ushort usLeakUnit;		// See F28_LEAK_UNITS
            public ushort usRejectCalc;	    // Pa or Pa/s
            public ushort usVolumeUnit;	    // See F28_ENUM_VOLUME_UNIT 
            public float fVolume;
            public float fRejectMin;
            public float fRejectMax;
            public float fCoeffAutoFill;
            public ushort usOptions;		// option 
            public float fPatmSTD;		    // Patm  standard condition (hPa)
            public float fTempSTD;		    // Temperature standard condition (en °C)
            public float fFilterTime;	    // in (s)
            public float fOffsetLeak;	    // Offset sur la fuite
            public float fVolumeRef;
            public ushort wTpsTestCompTemp;
            public ushort wPourcCompTemp;
            public ushort wTpsWaitingTime;
            public ushort wLastConsigneDacEasy;
            public float fNominalValue;
            public float fCoeffMin;
            public float fCoeffMax;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct F28_REALTIME_CYCLE
        {
            public byte bEndCycle;
            public byte bStatus;
            public float fPressureValue;
            public float fLeakValue;
            public byte bUnitPressure;
            public byte bUnitLeak;
            public float fInternalTemperature;
            public float fPatm;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct F28_RESULT
        {
            public byte bStatus;
            public float fPressureValue;
            public float fLeakValue;
            public byte bUnitPressure;
            public byte bUnitLeak;
            public byte bGroupID;
            public byte bModuleAddr;
            public F28_DATE dateReceived;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct F28_CYCLE_STATISTICS
        {
            public uint uiTotalCycles;
            public uint uiFailCycles;
            public uint uiSuccessCycles;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct F28_COMMUNICATION_STATISTICS
        {
            public uint uiTransmited;
            public uint uiReceived;
            public uint uiErrors;
        };

        // 1.500 Type of test
        public enum F28_TYPE_TEST : byte			// Uses with "wTypeTest" parameter
        {
            UNDEFINED_TEST,
            LEAK_TEST,
            SEALED_COMPONENT_TEST,
            DESENSITIZED_TEST                       // 1.500 Test type
        };

        public enum F28_ENUM_VOLUME_UNIT : byte	// Uses with "wVolumeUnit" parameter
        {
            VOLUME_CM3,
            VOLUME_MM3,
            VOLUME_ML,
            VOLUME_LITRE,
            VOLUME_INCH3,
            VOLUME_FT3,
            NMAX_VOLUME_UNITS
        };


        public enum F28_PRESS_UNITS : byte
        {
            PRESS_PA,
            PRESS_KPA,
            PRESS_MPA,
            PRESS_BAR,
            PRESS_mBAR,
            PRESS_PSI,
            PRESS_POINTS,
            NMAX_PRESS_UNITS
        };
        public string[] F28_STR_PRESS_UNITS = { "Pa", "KPa", "MPa", "bar", "mbar", "PSI", "Points" };

        public enum F28_LEAK_UNITS : byte
        {
            LEAK_PA,
            LEAK_PASEC,
            LEAK_PA_HR,
            LEAK_PASEC_HR,
            LEAK_CAL_PA,
            LEAK_CAL_PASEC,
            LEAK_CCMIN,
            LEAK_CCSEC,
            LEAK_CCH,
            LEAK_MM3SEC,
            LEAK_CM3_SEC,
            LEAK_CM3_MIN,
            LEAK_CM3_H,
            LEAK_ML_SEC,
            LEAK_ML_MIN,
            LEAK_ML_H,
            LEAK_INCH3_SEC,
            LEAK_INCH3_MIN,
            LEAK_INCH3_H,
            LEAK_FT3_SEC,
            LEAK_FT3_MIN,
            LEAK_FT3_H,
            LEAK_MMCE,
            LEAK_MMCE_SEC,
            LEAK_SCCM,
            LEAK_POINTS,
            // 1.500 Leak units
            LEAK_KPA,
            LEAK_MPA,
            LEAK_BAR,
            LEAK_mBAR,
            LEAK_PSI,
            LEAK_L_MIN,

            // V2.006
            LEAK_CM_H2O,

            LEAK_UG_H2O,

            NMAX_LEAK_UNITS,

            // V2.0.04
            LEAK_JET_CHECK = 0xff		// F28 check jet unit

        };
        public string[] F28_STR_LEAK_UNITS =
        {
            "Pa",
            "Pa/s",
            "Pa(Hr)",
            "Pa/s(Hr)",
            "Cal Pa",
            "Cal Pa/s",
            "cc/min",
            "cc/s",
            "cc/h",
            "mm3/s",
            "cm3/min",
            "cm3/s",
            "cm3/h",
            "ml/s",
            "ml/min",
            "ml/h",
            "in3/s",
            "in3/min",
            "in3/h",
            "Reserved1",
            "Reserved2",
            "Reserved3",
            "Reserved4",
            "Reserved5",
            "sccm",
            "Points",
            // 1.500 Leak units
            "kPa",
            "MPa",
            "bar",
            "mBar",
            "PSI",
            "l/mn",
            // V2.006
            "cmH2O",
            "ugH2O",
        };

        // V2.0.0.4
        public string F28_STR_LEAK_JET_CHECK = "mm";
        // V2.0.0.4

        public enum F28_STATUS : byte
        {
            F28_STATUS_READY,
            F28_STATUS_FILL,
            F28_STATUS_STAB,
            F28_STATUS_TEST,
            F28_STATUS_DUMP,
            F28_STATUS_FILL_VOLUME,
            F28_STATUS_TRANSFERT,
            F28_STATUS_MAX,
        };

        public enum F28_ENUM_FILL_MODE : byte
        {
            STD_FILL_MODE,
            AUTOFILL_MODE,
            INSTRUCTION_MODE,
            RAMP_MODE,
            RAMP_CONTROL,
            EASY,
            EASY_AUTO,
            NMAX_FILL_MODE
        };

        public string[] F28_STR_STATUS = { "READY", "FILL", "STABILIZATION", "TEST", "DUMP", "FILL VOLUME", "TRANSFERT" };

        public enum F28_RESULT_STATUS_CODE : byte
        {
            STATUS_GOOD_PART,
            STATUS_TEST_FAIL_PART,
            STATUS_REF_FAIL_PART,
            STATUS_ALARM_EEEE,
            STATUS_ALARM_MMMM,
            STATUS_ALARM_PPPP,
            STATUS_ALARM_MPPP,
            STATUS_ALARM_OFFD_LEAK,
            STATUS_ALARM_OFFD_PRESSURE,
            STATUS_ALARM_PST,
            STATUS_ALARM_MPST,
            STATUS_ALARM_CS_VOLUME_LOW,
            STATUS_ALARM_CS_VOLUME_HIGH,
            STATUS_ALARM_ERROR_PRESS_CALIBRATION,
            STATUS_ALARM_ERROR_LEAK_CALIBRATION,
            STATUS_ALARM_ERROR_LINE_PRESS_CALIBRATION,
            STATUS_ALARM_APPR_REG_ELEC_ERROR,
            STATUS_ALARM_TEST_PART_LARGE_LEAK,
            STATUS_ALARM_REF_SIDE_LARGE_LEAK,
            STATUS_ALARM_P_TOO_LARGE_FILL,
            STATUS_ALARM_P_TOO_LOW_FILL,
            STATUS_ALARM_JET_CHECK_FAIL,
            STATUS_ALARM_JET_CHECK_PASS,
            STATUS_ALARM_INCOMPATIBLE_DEVICE,
            NMAX_STATUS_CODE
        };

        // **************************************************************************************  
        // Result code
        // **************************************************************************************  
        /*
        STATUS_GOOD_PART							PASS
        STATUS_TEST_FAIL_PART						TEST FAIL
        STATUS_REF_FAIL_PART						REF FAIL
        STATUS_ALARM_EEEE							TEST OVER FULL SCALE : GROSS LEAK
        STATUS_ALARM_MMMM							REF OVER FULL SCALE : GROSS LEAK
        STATUS_ALARM_PPPP							PRESS.SIGNAL at VMax : PRESS.SENSOR DEFAULT
        STATUS_ALARM_MPPP							PRESS.SIGNAL at Vmin : PRESS.SENSOR DEFAULT
        STATUS_ALARM_OFFD_							FUITE DIFF AZ DEFAULT : DIFF SENSOR DEFAULT
        STATUS_ALARM_OFFD_PRESSION					PRESS AZ DEFAULT : PRESS SENSOR DEFAULT
        STATUS_ALARM_PST							PRESSURE TOO HIGH
        STATUS_ALARM_MPST							PRESSURE TOO LOW
        STATUS_ALARM_CS_VOLUME_PETIT				SEALED VOLUME TOO LOW
        STATUS_ALARM_CS_VOLUME_GRAND				FAIL SEALED VOLUME(TOO HIGH)
        STATUS_ALARM_ERROR_PRESS_CALIBRATION		DEF PRESS CALIB
        STATUS_ALARM_ERROR_LEAK_CALIBRATION			DEF DIFF CALIB
        STATUS_ALARM_ERROR_LINE_PRESS_CALIBRATION	DEF LINE CALIB
        */


        public string[] F28_RESULT_STR_STATUS =
        {
            "Pass",
            "Fail Test",
            "Fail Ref.",
            "TEST OVER FULL SCALE",
            "REF OVER FULL SCALE",
            "PRESS.SIGNAL at VMax",
            "PRESS.SIGNAL at Vmin",
            "FUITE DIFF AZ DEFAULT",
            "PRESS AZ DEFAULT",
            "PRESSURE TOO HIGH",
            "PRESSURE TOO LOW",
            "SEALED VOLUME TOO LOW",
            "FAIL SEALED VOLUME(TOO HIGH)",
            "DEF PRESS CALIBRATION",
            "DEF Leak Calibration",
            "DEF Line Pres.CAL",
            "ELECTRONIC REG LEARNING FAIL",
            "TEST PART LARGE LEAK ALARM",
            "REFERENCE SIDE LARGE LEAK ALARM",
            "PRESSURE TOO HIGH FILL TIME",
            "PRESSURE TOO LOW FILL TIME",
            "JET CHECK FAIL",
            "JET CHECK PASS",
            "INCOMPATIBLE DEVICE",
        };

        [DllImport(strDllName)]
        private static extern short F28_Init();

        [DllImport(strDllName)]
        private static extern ushort F28_GetDllMajorVersion();

        [DllImport(strDllName)]
        private static extern ushort F28_GetDllMinorVersion();

        [DllImport(strDllName)]
        private static extern short F28_OpenChannel();

        [DllImport(strDllName)]
        private static extern void F28_Close();

        [DllImport(strDllName)]
        private static extern short F28_AddModule(uint ulIP, byte bMoubleAddr, byte bGroupID, byte bTimeout);

        [DllImport(strDllName)]
        private static extern short F28_ReconnectModule(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_RemoveModule(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_RemoveAllModules();

        // 1.404 New
        [DllImport(strDllName)]
        private static extern short F28_ResetEthernetModule(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_RefreshModuleInformations(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_GetSerialNumber(short sModuleID, StringBuilder strSerialNumber, ushort usLength);

        [DllImport(strDllName, CharSet = CharSet.Ansi)]
        public static extern short F28_GetModuleSoftVersion(short sModuleID, StringBuilder strVersion, ushort usLength);

        [DllImport(strDllName, CharSet = CharSet.Ansi)]
        private static extern short F28_GetModuleHardVersion(short sModuleID, StringBuilder strVersion, ushort usLength);

        [DllImport(strDllName)]
        //      private static extern short F28_GetAddressIP(short sModuleID, ref ulong ulAddressIP);
        private static extern short F28_GetAddressIP(short sModuleID, ref uint ulAddressIP);                // 1.504

        [DllImport(strDllName)]
        private static extern short F28_GetETHSoftVersion(short sModuleID, StringBuilder strVersion, ushort usLength);

        [DllImport(strDllName)]
        private static extern short F28_GetETHHardVersion(short sModuleID, StringBuilder strVersion, ushort usLength);

        [DllImport(strDllName)]
        //      private static extern short F28_GetSubnetMask(short sModuleID, ref ulong ulAddressIP);
        private static extern short F28_GetSubnetMask(short sModuleID, ref uint ulAddressIP);               // 1.504

        [DllImport(strDllName)]
        //      private static extern short F28_GetGatewayAddressIP(short sModuleID, ref ulong ulAddressIP);
        private static extern short F28_GetGatewayAddressIP(short sModuleID, ref uint ulAddressIP);         // 1.504

        [DllImport(strDllName)]
        private static extern short F28_GetMACAddress(short sModuleID, StringBuilder strMAC, ushort usLength);

        [DllImport(strDllName)]
        private static extern short F28_IsModuleConnected(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_StartCycle(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_StopCycle(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_StartCycleByGroup(byte bGroupID);

        [DllImport(strDllName)]
        private static extern short F28_StopCycleByGroup(byte bGroupID);

        [DllImport(strDllName)]
        private static extern short F28_GetModuleParameters(short sModuleID, ref F28_PARAMETERS tPara);

        [DllImport(strDllName)]
        private static extern short F28_SetModuleParameters(short sModuleID, ref F28_PARAMETERS tPara);

        [DllImport(strDllName)]
        private static extern short F28_GetRealTimeData(short sModuleID, ref F28_REALTIME_CYCLE tCycle);


        [DllImport(strDllName)]
        private static extern short F28_ClearFIFOResults(short sModuleID);

        [DllImport(strDllName)]
        private static extern ushort F28_GetResultsCount(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_GetNextResult(short sModuleID, ref F28_RESULT tResult);

        [DllImport(strDllName)]
        private static extern short F28_GetLastResult(short sModuleID, ref F28_RESULT tResult);

        [DllImport(strDllName)]
        private static extern short F28_GetCycleStatistics(short sModuleID, ref F28_CYCLE_STATISTICS tInfo);

        [DllImport(strDllName)]
        private static extern short F28_GetCommunicationStatistics(short sModuleID, ref F28_COMMUNICATION_STATISTICS tInfo);

        // ***********************************************************************************************
        // Special Cycles
        // ***********************************************************************************************
        // 1.402 Functions

        [DllImport(strDllName)]
        private static extern short F28_StartAutoZeroPressure(short sModuleID, float fDumpTime);

        [DllImport(strDllName)]
        private static extern short F28_StartRegulatorAdjust(short sModuleID);

        // 1.500 Electronic Regulator Learn
        [DllImport(strDllName)]
        private static extern short F28_StartLearningRegulator(short sModuleID, float fDumpTime);

        // Auto calibration
        [DllImport(strDllName)]
        private static extern byte F28_GetEOCOffset(short sModuleID);

        [DllImport(strDllName)]
        private static extern byte F28_GetEOCVolume(short sModuleID);

        [DllImport(strDllName)]
        private static extern byte F28_GetEOCRatio(short sModuleID);

        [DllImport(strDllName)]
        private static extern byte F28_GetEOCEasyAutoLearning(short sModuleID);

        [DllImport(strDllName)]
        private static extern byte F28_GetAutoCalAlarm(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_StartAutoCalOffsetOnly(short sModuleID, ushort wNbCycles, ushort wInterCycleTime, float fOffsetMax);

        [DllImport(strDllName)]
        private static extern short F28_StartAutoCalOffset(short sModuleID, ushort wNbCycles, ushort wInterCycleTime, float fOffsetMax);

        [DllImport(strDllName)]
        private static extern short F28_StartAutoCalVolume(short sModuleID, ushort wNbCycles, ushort wInterCycleTime, float fLeak, float fPressure, float fVolMin, float fVolMax);

        [DllImport(strDllName)]
        private static extern short F28_StartAutoRatio(short sModuleID, ushort wNbCycles, ushort wInterCycleTime, float fRatioMax, float fRatioMin);

        [DllImport(strDllName)]
        private static extern short F28_StartEasyAutoLearning(short sModuleID, ushort wNbCycles, ushort wInterCycleTime);

        [DllImport(strDllName)]
        private static extern short F28_StopAutoCal(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_StopAutoRatio(short sModuleID);

        [DllImport(strDllName)]
        private static extern short F28_StopEasyAutoLearning(short sModuleID);

        // V2.0.0.4 Jet check
        [DllImport(strDllName)]
        private static extern short F28_StartJetCheck(short sModuleID);
        // End 

        // ***********************************************************************************************
        // Wrapper functions
        // ***********************************************************************************************
        //将dll函数进行封装

        public short Init()
        {
            short sRet = F28_FAIL;
            try
            {
                new UIPermission(UIPermissionWindow.AllWindows).Demand();
                sRet = F28_Init();
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Error interacting with F28LightControl_ETH.dll : " + e.Message, @" DLL Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            return sRet;
        }

        public ushort GetDllMajorVersion()
        {
            return F28_GetDllMajorVersion();
        }

        public ushort GetDllMinorVersion()
        {
            return F28_GetDllMinorVersion();
        }

        public short OpenChannel()
        {
            return F28_OpenChannel();
        }

        public void close()
        {
            F28_Close();
        }

        public short AddModule(uint ulIP, byte bModuleAddr, byte bGroupID, byte bTimeout)
        {
            return F28_AddModule(ulIP, bModuleAddr, bGroupID, bTimeout);
        }

        public short ReconnectModule(short sModuleID)
        {
            return F28_ReconnectModule(sModuleID);
        }

        public short RemoveModule(short sModuleID)
        {
            return F28_RemoveModule(sModuleID);
        }

        // 1.404
        public short ResetEthernetModule(short sModuleID)
        {
            return F28_ResetEthernetModule(sModuleID);
        }

        public short RefreshModuleInformations(short sModuleID)
        {
            return F28_RefreshModuleInformations(sModuleID);
        }

        public short GetSerialNumber(short sModuleID,ref string strSerialNumber)
        {
            short sRet = F28_FAIL;
            StringBuilder Str = new StringBuilder(usSizeSerialNumber);
            sRet = F28_GetSerialNumber(sModuleID, Str, (ushort)Str.Capacity);

            strSerialNumber = Str.ToString();
            return sRet;
        }

        public short GetModuleSoftVersion(short sModuleID, ref string strVersion)
        {
            short sRet = F28_FAIL;
            StringBuilder Str = new StringBuilder(usSizeVersion);

            sRet = F28_GetModuleSoftVersion(sModuleID, Str, (ushort)Str.Capacity);
            strVersion = Str.ToString();

            return sRet;
        }

        public short GetModuleHardVersion(short sModuleID, ref string strVersion)
        {
            short sRet;
            StringBuilder Str = new StringBuilder(usSizeVersion);

            sRet = F28_GetModuleHardVersion(sModuleID, Str, (ushort)Str.Capacity);

            strVersion = Str.ToString();

            return sRet;
        }

        // 1.504
        public short GetSubnetMask(short sModuleID, ref uint ulAddressIP)
        {
            return F28_GetSubnetMask(sModuleID, ref ulAddressIP);
        }

        public short GetMACAddress(short sModuleID, ref string strMAC)
        {
            short sRet = F28_FAIL;
            StringBuilder Str = new StringBuilder(32);

            sRet = F28_GetMACAddress(sModuleID, Str, (ushort)Str.Capacity);
            strMAC = Str.ToString();

            return sRet;
        }

        public short IsModuleConnected(short sModuleID)
        {
            return F28_IsModuleConnected(sModuleID);
        }

        public short StartCycle(short sModuleID)
        {
            return F28_StartCycle(sModuleID);
        }

        public short StopCycle(short sModuleID)
        {
            return F28_StopCycle(sModuleID);
        }

        public short StartCycleByGroup(byte bGroupID)
        {
            return F28_StartCycleByGroup(bGroupID);
        }

        public short StopCycleByGroup(byte bGroupID)
        {
            return F28_StopCycleByGroup(bGroupID);
        }

        public short GetModuleParameters(short sModuleID, ref F28_PARAMETERS tPara)
        {
            return F28_GetModuleParameters(sModuleID, ref tPara);
        }

        public short SetModuleParameters(short sModuleID, ref F28_PARAMETERS tPara)
        {
            return F28_SetModuleParameters(sModuleID, ref tPara);
        }

        public short GetRealTimeData(short sModuleID, ref F28_REALTIME_CYCLE tCycle)
        {
            return F28_GetRealTimeData(sModuleID, ref tCycle);
        }

        public short ClearFIFOResults(short sModuleID)
        {
            return F28_ClearFIFOResults(sModuleID);
        }

        public ushort GetResultsCount(short sModuleID)
        {
            return F28_GetResultsCount(sModuleID);
        }

        public short GetNextResult(short sModuleID, ref F28_RESULT tResult)
        {
            return F28_GetNextResult(sModuleID, ref tResult);
        }

        public short GetLastResult(short sModuleID, ref F28_RESULT tResult)
        {
            return F28_GetLastResult(sModuleID, ref tResult);
        }

        public short GetCycleStatistics(short sModuleID, ref F28_CYCLE_STATISTICS tInfo)
        {
            return F28_GetCycleStatistics(sModuleID, ref tInfo);
        }

        public short GetCommunicationStatistics(short sModuleID, ref F28_COMMUNICATION_STATISTICS tInfo)
        {
            return F28_GetCommunicationStatistics(sModuleID, ref tInfo);
        }

        // ***********************************************************************************************
        // Special Cycles
        // ***********************************************************************************************

        public short StartAutoZeroPressure(short sModuleID, float fDumpTime)
        {
            return F28_StartAutoZeroPressure(sModuleID, fDumpTime);
        }

        public short StartRegulatorAdjust(short sModuleID)
        {
            return F28_StartRegulatorAdjust(sModuleID);
        }

        // 1.500 Electronic Regulator Learn
        public short StartLearningRegulator(short sModuleID, float fDumpTime)
        {
            return F28_StartLearningRegulator(sModuleID, fDumpTime);
        }

        // ***********************************************************************************************
        // Auto Offset + Volume Calibration
        // ***********************************************************************************************

        public byte GetEOCOffset(short sModuleID)
        {
            return F28_GetEOCOffset(sModuleID);
        }

        public byte GetEOCVolume(short sModuleID)
        {
            return F28_GetEOCVolume(sModuleID);
        }

        public byte GetEOCRatio(short sModuleID)
        {
            return F28_GetEOCRatio(sModuleID);
        }

        public byte GetEOCEasyAutoLearning(short sModuleID)
        {
            return F28_GetEOCEasyAutoLearning(sModuleID);
        }

        public byte GetAutoCalAlarm(short sModuleID)
        {
            return F28_GetAutoCalAlarm(sModuleID);
        }

        public short StartAutoCalOffsetOnly(short sModuleID, ushort wNbCycles, ushort wInterCycleTime, float fOffsetMax)
        {
            return F28_StartAutoCalOffsetOnly(sModuleID, wNbCycles, wInterCycleTime, fOffsetMax);
        }

        public short StartAutoCalOffset(short sModuleID, ushort wNbCycles, ushort wInterCycleTime, float fOffsetMax)
        {
            return F28_StartAutoCalOffset(sModuleID, wNbCycles, wInterCycleTime, fOffsetMax);
        }

        public short StartAutoCalVolume(short sModuleID, ushort wNbCycles, ushort wInterCycleTime, float fLeak, float fPressure, float fVolMin, float fVolMax)
        {
            return F28_StartAutoCalVolume(sModuleID, wNbCycles, wInterCycleTime, fLeak, fPressure, fVolMin, fVolMax);
        }

        public short StartAutoRatio(short sModuleID, ushort wNbCycles, ushort wInterCycleTime, float fRatioMax, float fRatioMin)
        {
            return F28_StartAutoRatio(sModuleID, wNbCycles, wInterCycleTime, fRatioMax, fRatioMin);
        }

        public short StartEasyAutoLearning(short sModuleID, ushort wNbCycles, ushort wInterCycleTime)
        {
            return F28_StartEasyAutoLearning(sModuleID, wNbCycles, wInterCycleTime);
        }

        public short StopAutoCal(short sModuleID)
        {
            return F28_StopAutoCal(sModuleID);
        }

        public short StopAutoRatio(short sModuleID)
        {
            return F28_StopAutoRatio(sModuleID);
        }

        public short StopEasyAutoLearning(short sModuleID)
        {
            return F28_StopEasyAutoLearning(sModuleID);
        }

        // V2.0.0.4 Start jet check
        public short StartJetCheck(short sModuleID)
        {
            return F28_StartJetCheck(sModuleID);
        }

    }
}
