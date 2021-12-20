openAuditTrailCommentForExport() {

  if (this.currentRows.length < 1) {
    this.noRowsFoundPopup();
  }
  else {
    this.checkForUSBs();
  }
}

noRowsFoundPopup() {
  const warning = this.dataSharing.translateMessage(
    "resultsTable.exportNoRows"
  );
  const ok: string = this.dataSharing.translateMessage("buttonText.ok");

  const dialogWarning = this.dialog.open(WarningPopupComponent, {
    data: {
      message: warning,
      button1Text: ok,
      button2Text: "",
      button3Text: "",
    },
  });
}

checkForUSBs() {
  this.apiRequest.checkIfUSBIsPresent().subscribe(
    (data: any) => {
      if (data.value.status !== "") {
        // No usb devices found
        this.noUSBFoundPopup(); 
      } 
      else {
        this.checkForDuplicates();
      }
    },
    (err) => {}
  );
}

noUSBFoundPopup() {
  const dialogError = this.dialog.open(ErrorPopupComponent, {
    data: { message: this.dataSharing.translateMessage("logs.noUsb") },
  });
}

checkForDuplicates() {
  const dataToExport = {
    fileNames: this.filenames,
  };
  this.apiRequest
          .checkIfDuplicateFilesArePresent(dataToExport)
          .subscribe(
            (dataDuplicate: any) => {
              if (dataDuplicate.value.isDuplicatesresent === "1") {
                // Duplicate files present
                this.duplicatesFoundPopup();
              } else {
                this.addToAuditTrail();
              }
            },
            (err) => {}
          );
}

duplicatesFoundPopup() {
  const ok: string =
    this.dataSharing.translateMessage("buttonText.ok");
  const cancel: string =
    this.dataSharing.translateMessage("buttonText.cancel");
  const dialogWarning = this.dialog.open(
    WarningPopupComponent,
    {
      data: {
        message:
          this.dataSharing.translateMessage("logs.duplicate"),
        button1Text: ok,
        button2Text: cancel,
        button3Text: "",
      },
    }
  );
}

addToAuditTrail() {
  const dialogComment = this.dialog.open(
    AuditTrailCommentComponent,
    { data: { value: "ExportLogs" } }
  );
  dialogComment.afterClosed().subscribe((closed) => {
    if (closed != undefined) {
      if (closed) {
        this.exportClicked();
      }
    }
  });
}