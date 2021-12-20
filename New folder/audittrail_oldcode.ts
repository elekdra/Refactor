openAuditTrailCommentForExport() {
  if (this.currentRows.length < 1) {
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

    return;
  }

  this.apiRequest.checkIfUSBIsPresent().subscribe(
    (data: any) => {
      if (data.value.status !== "") {
        // No usb devices found

        const dialogError = this.dialog.open(ErrorPopupComponent, {
          data: { message: this.dataSharing.translateMessage("logs.noUsb") },
        });
      } else {
        const dataToExport = {
          fileNames: this.filenames,
        };
        this.apiRequest
          .checkIfDuplicateFilesArePresent(dataToExport)
          .subscribe(
            (dataDuplicate: any) => {
              if (dataDuplicate.value.isDuplicatesresent === "1") {
                // Duplicate files present
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
              } else {
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
            },
            (err) => {}
          );
      }
    },
    (err) => {}
  );
}