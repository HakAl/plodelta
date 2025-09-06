// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import {Fragment} from "react";
import TheReport from "./Home_Saved Reports_GTOStatAnalyzer.report";

function Instructions({selectedReport}) {
    const reportToDownload = TheReport;
    return <Fragment>
        <h3>How To</h3>
        <p>This guide assumes you have saved hand histories imported into Holdem Manager 3 (HM3).</p>

        <a href="/plodelta/releases/latest/download/PlodeltaImport-win-x64.exe" download>
            Download Windows helper
        </a>

        <ol>
            <li>Open HM3.</li>
            <li>In the reports section, select the new GTOStatAnalyzer report.</li>
            <li>Right click the stats, "Select All".</li>
            <li>Right click the stats, "Save As".</li>
            <li>Use the button below to upload the saved file.</li>
        </ol>
    </Fragment>
}

export default Instructions;