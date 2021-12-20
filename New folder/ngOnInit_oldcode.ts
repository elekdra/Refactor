ngOnInit() {
    this.temperatureData = [
      { name: "dashboard.combustionFurnace", setValue: 750, currentValue: 0 },
      { name: "dashboard.reductionFurnace", setValue: 450, currentValue: 0 },
      { name: "dashboard.degassingFurnace", setValue: 330, currentValue: 0 },
      { name: "dashboard.detector", setValue: 60.0, currentValue: 0 },
    ];
    this.pressureData = [{ name: "dashboard.system", currentValue: 0 }];
    this.modeTimeData = [
      { name: "dashboard.workMode", value: 1862 },
      { name: "dashboard.standbyMode", value: 4468 },
      { name: "dashboard.overall", value: 8744 },
    ];
    this.flowData = [
      { name: "dashboard.hefc1", setValue: 20.0, currentValue: 0 },
      { name: "dashboard.hefc2", setValue: 20.0, currentValue: 0 },
      { name: "dashboard.o2", setValue: 0.0, currentValue: 0 },
    ];
    this.detectorData = [
      { name: "dashboard.volt", value: 0.0, units: "V" },
      { name: "dashboard.drift", value: 0.0, units: "mV/min" },
      { name: "dashboard.derivative", value: 0.0, units: "mV/s" },
    ];

    this.getDriftLimits();
    
    this.temperatureData[3].setValue = Number(
      this.temperatureData[3].setValue
    ).toFixed(1);
    this.flowData[2].setValue = Number(this.flowData[0].setValue).toFixed(1);
    this.flowData[2].currentValue = Number(
      this.flowData[1].currentValue
    ).toFixed(1);

    this.apiRequest.getModeDataValues().subscribe(
      (data: any) => {
        if (data.value.dataModel !== null) {
          var modeData = data.value.dataModel.mode;
          this.dataSharing.deviceModeStatus = modeData;

          var modeData = data.value.dataModel.modeData;
          this.temperatureData[0].setValue = Number(
            Math.floor(modeData.cFsetpoint)
          ).toFixed(0);
          this.temperatureData[1].setValue = Number(
            Math.floor(modeData.rFsetpoint)
          ).toFixed(0);
          this.temperatureData[2].setValue = Number(
            Math.floor(modeData.dFsetpoint)
          ).toFixed(0);

          this.flowData[0].setValue = Number(modeData.fC1flowrate).toFixed(1);
          this.flowData[1].setValue = Number(modeData.fC2flowrate).toFixed(1);
        }
      },
      (err) => {
        console.log("ERROR......" + err.message);
      }
    );

    this.furnaceData$ = this.store.pipe(select(selectFurnaceData));
    this.furnaceDataSubscription = this.furnaceData$.subscribe((data) => {
      let cfDiff = Math.abs(
        this.temperatureData[0].setValue - data.combustionFurnaceTemp
      );
      let rfDiff = Math.abs(
        this.temperatureData[1].setValue - data.reductionFurnaceTemp
      );
      let dfDiff = Math.abs(
        this.temperatureData[2].setValue - data.degassingFurnaceTemp
      );
      //if the temperature differences is 0.5 take the ceil value otherwise the floor value
      this.temperatureData[0].currentValue =
        data.combustionFurnaceTemp == 0
          ? Number(Math.floor(this.temperatureData[0].currentValue)).toFixed(0)
          : cfDiff <= 0.5
          ? Number(Math.ceil(data.combustionFurnaceTemp).toFixed(0))
          : Number(Math.floor(data.combustionFurnaceTemp)).toFixed(0);
      this.temperatureData[1].currentValue =
        data.reductionFurnaceTemp == 0
          ? Number(Math.floor(this.temperatureData[1].currentValue)).toFixed(0)
          : rfDiff <= 0.5
          ? Number(Math.ceil(data.reductionFurnaceTemp).toFixed(0))
          : Number(Math.floor(data.reductionFurnaceTemp)).toFixed(0);
      this.temperatureData[2].currentValue =
        data.degassingFurnaceTemp == 0
          ? Number(Math.floor(this.temperatureData[2].currentValue)).toFixed(0)
          : dfDiff <= 0.5
          ? Number(Math.ceil(data.degassingFurnaceTemp).toFixed(0))
          : Number(Math.floor(data.degassingFurnaceTemp)).toFixed(0);
      this.temperatureData[3].currentValue =
        data.tcdTemp == 0
          ? Number(this.temperatureData[3].currentValue).toFixed(1)
          : Number(data.tcdTemp).toFixed(1);
    });

    this.flowControllerData$ = this.store.pipe(
      select(selectFlowControllerData)
    );

    this.flowControllerDataSubscription = this.flowControllerData$.subscribe(
      (data) => {
        if (data.heFlow) {
          this.flowData[0].currentValue =
            data.flowController1 == 0
              ? Number(this.flowData[0].currentValue).toFixed(1)
              : Number(data.flowController1).toFixed(1);
          this.flowData[1].currentValue =
            data.flowController2 == 0
              ? Number(this.flowData[1].currentValue).toFixed(1)
              : Number(data.flowController2).toFixed(1);
          this.flowData[2].currentValue = 0.0;
          this.flowData[2].setValue = 0.0;
        } else {
          this.flowData[0].currentValue = 0.0;
          this.flowData[1].currentValue =
            data.flowController2 == 0
              ? Number(this.flowData[1].currentValue).toFixed(1)
              : Number(data.flowController2).toFixed(1);
          this.flowData[2].currentValue =
            data.flowController1 == 0
              ? Number(this.flowData[0].currentValue).toFixed(1)
              : Number(data.flowController1).toFixed(1);
          this.flowData[2].setValue = Number(data.o2).toFixed(1);
        }
      }
    );
    this.detectorData$ = this.store.pipe(select(selectDetectorData));
    this.detectorDataSubscription = this.detectorData$.subscribe((data) => {
      this.detectorData[0].value =
        data.tcdVol == 0
          ? Number(this.detectorData[0].value).toFixed(3)
          : Number(data.tcdVol).toFixed(3);
      this.detectorData[1].value =
        data.driftLimit == 0
          ? Number(this.detectorData[1].value).toFixed(1)
          : Number(data.driftLimit).toFixed(1);
      this.detectorData[2].value =
        data.derivativeLimit == 0
          ? Number(this.detectorData[2].value).toFixed(3)
          : Number(data.derivativeLimit).toFixed(3);
    });

    this.pressureData$ = this.store.pipe(select(selectPressureData));

    this.apiRequest.getMaintenanceInfo().subscribe(
      (data: any) => {
        if (data.value.pParams !== null) {
          let info = data.value.pParams;
          this.modeTimeData[0].value = info.workruntime;
          this.modeTimeData[1].value = info.standbyruntime;
          this.modeTimeData[2].value = info.totalrun;
        }
      },
      (err) => {}
    );

    this.modeSubscription = this.dataSharing.modeChanged.subscribe(() => {
      if (
        this.dataSharing.deviceModeStatus === undefined ||
        this.dataSharing.deviceModeStatus === ""
      ) {
        return;
      }
      this.apiRequest.getModeData(this.dataSharing.deviceModeStatus).subscribe(
        (data: any) => {
          if (data.value.modeData !== null) {
            let modeData = data.value.modeData;
            this.temperatureData[0].setValue = Number(
              Math.floor(modeData.cFsetpoint)
            ).toFixed(0);
            this.temperatureData[1].setValue = Number(
              Math.floor(modeData.rFsetpoint)
            ).toFixed(0);
            this.temperatureData[2].setValue = Number(
              Math.floor(modeData.dFsetpoint)
            ).toFixed(0);

            this.flowData[0].setValue = Number(modeData.fC1flowrate).toFixed(1);
            this.flowData[1].setValue = Number(modeData.fC2flowrate).toFixed(1);
          }
        },
        (err) => {
          console.log("ERROR......" + err.message);
        }
      );
    });
}