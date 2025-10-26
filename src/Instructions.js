// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import {Fragment} from "react";
import TheReport from "./Home_Saved Reports_GTOStatAnalyzer.report";

function Instructions({selectedReport}) {
    const reportToDownload = TheReport;
    return (
        <div className="body-instructions">
            <h2 className="mb-4">Analyze your poker stats with <a href="https://www.holdemmanager.com/hm3/download.php" rel="noreferrer" className="link" target="_blank">Holdem Manager 3</a></h2>

            <h3 className="mb-3">Getting Started</h3>
            <p>Make sure you have hand histories imported into Holdem Manager 3 before beginning.</p>

            <ol>
                <li>
                    <a
                        href="/plodelta/releases/latest/download/PlodeltaImport-win-x64.exe"
                        download
                        className="btn btn-link link p-0"
                    >
                        Download the HM3 report template
                    </a>
                </li>
                <li>Open Holdem Manager 3</li>
                <li>Go to the Reports section and select the GTOStatAnalyzer report</li>
                <li>Right-click the stats table and choose "Select All"</li>
                <li>Right-click again and choose "Save As" to export your data</li>
                <li>Upload the saved file using the button below</li>
            </ol>
        </div>
    );
}

export default Instructions;