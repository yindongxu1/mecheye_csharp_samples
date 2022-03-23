using System;
using System.Collections.Generic;
using mmind.apiSharp;

class sample
{
    static bool isNumber(String str)
    {
        foreach(char c in str)
        {
            if (c >= '0' && c <= '9')
                return true;
        }
        return false;
    }

    static void showError(ErrorStatus status)
    {
        if (status.errorCode == (Int32)ErrorCode.MMIND_STATUS_SUCCESS)
            return;
        Console.WriteLine("Error Code : {0}, Error Description: {1}.", status.errorCode, status.errorDescription);
    }

    static void printDeviceInfo(MechEyeDeviceInfo deviceInfo)
    {
        Console.WriteLine("............................");
        Console.WriteLine("Camera Model Name: {0}", deviceInfo.model);
        Console.WriteLine("Camera ID:         {0}", deviceInfo.id);
        Console.WriteLine("Camera IP Address: {0}", deviceInfo.ipAddress);
        Console.WriteLine("Hardware Version:  V{0}", deviceInfo.hardwareVersion);
        Console.WriteLine("Firmware Version:  V{0}", deviceInfo.firmwareVersion);
        Console.WriteLine("............................");
        Console.WriteLine("");
    }

    static int Main()
    {
        Console.WriteLine("Find Mech-Eye device :");
        List<MechEyeDeviceInfo> deviceInfoList = MechEyeDevice.enumerateMechEyeDeviceList();

        if (deviceInfoList.Count == 0)
        {
            Console.WriteLine("No Mech-Eye Device found.");
            return -1;
        }

        for (int i = 0; i < deviceInfoList.Count; ++i)
        {
            Console.WriteLine("Mech-Eye device index : {0}", i);
            printDeviceInfo(deviceInfoList[i]);
        }

        Console.WriteLine("Please enter the device index you want to connect: ");
        Int32 inputIndex = 0;

        while (true)
        {
            String input = Console.ReadLine();
            if (Int32.TryParse(input, out inputIndex) && inputIndex < deviceInfoList.Count)
                break;
            Console.WriteLine("Input invalid! Please enter the device index you wnat to connect: ");
        }

        //MechEyeDeviceInfo deviceInfo = new MechEyeDeviceInfo() { model = "", id = "", hardwareVersion = "", firmwareVersion = "1.5.0", ipAddress = "127.0.0.1", port = 5577 };

        ErrorStatus status = new ErrorStatus();
        MechEyeDevice device = new MechEyeDevice();
        status = device.connect(deviceInfoList[inputIndex]);

        //status = device.connect(deviceInfo);

        if (status.errorCode != (Int32)ErrorCode.MMIND_STATUS_SUCCESS)
        {
            showError(status);
            return -1;
        }

        Console.WriteLine("Connect Mech-Eye Success.");

        MechEyeDeviceInfo deviceInfo = new MechEyeDeviceInfo();
        showError(device.getDeviceInfo(ref deviceInfo));
        printDeviceInfo(deviceInfo);

        List<String> userSets = new List<String>();
        showError(device.getAllUserSets(ref userSets));

        Console.WriteLine("All user sets : ");
        for (int i = 0; i < userSets.Count; ++i)
        {
            Console.Write(userSets[i]);
            Console.Write("  ");
        }
        Console.WriteLine("");

        String currentUserSet = "";
        showError(device.getCurrentUserSet(ref currentUserSet));
        Console.WriteLine("Current user set: {0}", currentUserSet);

        showError(device.setCurrentUserSet(userSets[0]));
        Console.WriteLine("Set \"{0}\" as the current user set.", userSets[0]);
        Console.WriteLine("");

        showError(device.setScan3DExposure(new List<Double>{5, 10}));

        List<Double> exposureSequence = new List<Double>();
        showError(device.getScan3DExposure(ref exposureSequence));

        Console.WriteLine("The 3D scanning exposure multiplier : {0}.", exposureSequence.Count);
        for (int i = 0; i < exposureSequence.Count; ++i)
            Console.WriteLine("3D scanning exposure time {0} : {1} ms.", i + 1, exposureSequence[i]);

        showError(device.setDepthRange(new DepthRange(100, 1000)));
        DepthRange depthRange = new DepthRange();
        showError(device.getDepthRange(ref depthRange));
        Console.WriteLine("3D Scanning depth lower limit : {0} mm, depth upper limit : {1} mm.", depthRange.lower, depthRange.upper);

        showError(device.setScan3DROI(new ROI(0, 0, 500, 500)));
        ROI scan3dRoi = new ROI();
        showError(device.getScan3DROI(ref scan3dRoi));
        Console.WriteLine("3D Scanning ROI topLeftX : {0}, topLeftY : {1}, width : {2}, height : {3}", scan3dRoi.x, scan3dRoi.y, scan3dRoi.width, scan3dRoi.height);

        showError(device.setScan2DExposureMode(Scan2DExposureMode.Timed));
        showError(device.setScan2DExposureTime(100));

        Scan2DExposureMode exposureMode2D = new Scan2DExposureMode();
        Double scan2DExposureTime = new Double();
        showError(device.getScan2DExposureMode(ref exposureMode2D));
        showError(device.getScan2DExposureTime(ref scan2DExposureTime));
        Console.WriteLine("2D scanning exposure mode enum : {0}, exposure time : {1} ms.", exposureMode2D, scan2DExposureTime);

        showError(device.setCloudSmoothMode(CloudSmoothMode.Normal));
        showError(device.setCloudOutlierFilterMode(CloudOutlierFilterMode.Normal));

        CloudSmoothMode cloudSmoothMode = new CloudSmoothMode();
        CloudOutlierFilterMode cloudOutlierFilterMode = new CloudOutlierFilterMode();
        showError(device.getCloudSmoothMode(ref cloudSmoothMode));
        showError(device.getCloudOutlierFilterMode(ref cloudOutlierFilterMode));

        Console.WriteLine("Cloud smooth mode enum : {0}, cloud outlier filter mode enum : {1}", cloudSmoothMode, cloudOutlierFilterMode);

        showError(device.saveAllSettingsToUserSets());
        Console.WriteLine("save all parammeters to current user set.");

        device.disconnect();
        Console.WriteLine("Disconnect Mech-Eye Success.");

        return 0;
    }
}