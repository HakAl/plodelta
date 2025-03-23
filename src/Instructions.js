import {Fragment} from "react";
import TheReport from "./Home_Saved Reports_GTOStatAnalyzer.report";

function Instructions({selectedReport}) {
    const reportToDownload = TheReport;
    return <Fragment>
        <h3>How To</h3>
        <p>Holdem Manager 3 (HM3) has a reports tool that can be used to export statistics, BUT
            there's no way to import reports into HM3, so you will have to download the report and add it to HM3 manually.</p>
        <p>HM3 reports are stored in a hidden directory. To show hidden files: <code>Open File Explorer, click on the View button, select Show > Hidden items</code></p>
        <ol>
            <li>Close HM3.</li>
            <li>
                <a href={reportToDownload}
                   target="_blank"
                   rel="noreferrer"
                   className={'link'}
                   download="Home_Saved Reports_GTOStatAnalyzer.report">Download the HM3 report file.</a></li>
            <li>Locate the HM3 install directory. EG: "C:\Users\YOUR_NAME\AppData\Roaming\Max Value Software\Holdem Manager\3.0\"</li>
            <li>Paste the downloaded file into the saved reports directory: EG: C:\Users\YOUR_NAME\AppData\Roaming\Max Value Software\Holdem Manager\3.0\Reports\Saved\</li>
            <li>Open HM3.</li>
            <li>In the reports section, select the new GTOStatAnalyzer report.</li>
            <li>Right click the stats, "Select All".</li>
            <li>Right click the stats, "Save As".</li>
            <li>Use the button below to upload the saved file.</li>
        </ol>
    </Fragment>
}

export default Instructions;