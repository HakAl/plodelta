import {Fragment} from "react";
import TheReport from "./Home_Saved Reports_GTOStatAnalyzer.report";

function Instructions({selectedReport}) {
    const reportToDownload = TheReport;
    return <Fragment>
        <h3>How To</h3>
        <p>This guide assumes you have saved hand histories imported into Holdem Manager 3 (HM3).</p>
        <p>HM3 Reports can be used to export your statistics. Download <a href={reportToDownload}
                                                                          target="_blank"
                                                                          rel="noreferrer"
                                                                          className={'link'}
                                                                          download="Home_Saved Reports_GTOStatAnalyzer.report">this
            file</a> and add it to HM3:</p>
        <p>HM3 reports are stored in a hidden directory. To show hidden files: <code>Open File Explorer, click on the
            View button, select Show > Hidden items</code></p>

        <div className="accordion accordion-flush" id="accordionFlushExample">
            <div className="accordion-item">
                <h2 className="accordion-header">
                    <button className="accordion-button collapsed" type="button" data-bs-toggle="collapse"
                            data-bs-target="#flush-collapseOne" aria-expanded="false" aria-controls="flush-collapseOne">
                        Install Holdem Manager 3
                    </button>
                </h2>
                <div id="flush-collapseOne" className="accordion-collapse collapse"
                     data-bs-parent="#accordionFlushExample">
                    <div className="accordion-body">
                        If you don't have or use HM3,

                        <a href={'https://www.holdemmanager.com/hm3/download.php'} rel="noreferrer" className={'link'}
                           target={'_blank'}>download and install it from here</a>
                        . Don't worry, there's a free trial.
                    </div>
                </div>
            </div>
            <div className="accordion-item">
                <h2 className="accordion-header">
                    <button className="accordion-button collapsed" type="button" data-bs-toggle="collapse"
                            data-bs-target="#flush-collapseTwo" aria-expanded="false" aria-controls="flush-collapseTwo">
                        Accordion Item #2
                    </button>
                </h2>
                <div id="flush-collapseTwo" className="accordion-collapse collapse"
                     data-bs-parent="#accordionFlushExample">
                    <div className="accordion-body">Placeholder content for this accordion, which is intended to
                        demonstrate the <code>.accordion-flush</code> class. This is the second item's accordion body.
                        Let's imagine this being filled with some actual content.
                    </div>
                </div>
            </div>
            <div className="accordion-item">
                <h2 className="accordion-header">
                    <button className="accordion-button collapsed" type="button" data-bs-toggle="collapse"
                            data-bs-target="#flush-collapseThree" aria-expanded="false"
                            aria-controls="flush-collapseThree">
                        Accordion Item #3
                    </button>
                </h2>
                <div id="flush-collapseThree" className="accordion-collapse collapse"
                     data-bs-parent="#accordionFlushExample">
                    <div className="accordion-body">Placeholder content for this accordion, which is intended to
                        demonstrate the <code>.accordion-flush</code> class. This is the third item's accordion body.
                        Nothing more exciting happening here in terms of content, but just filling up the space to make
                        it look, at least at first glance, a bit more representative of how this would look in a
                        real-world application.
                    </div>
                </div>
            </div>
        </div>


        <ol>
            <li>Install <a href={'https://www.holdemmanager.com/hm3/download.php'} rel="noreferrer" className={'link'}
                           target={'_blank'}>Holdem Manager 3</a>.
            </li>
            <li>Close HM3.</li>
            <li>
                <a href={reportToDownload}
                   target="_blank"
                   rel="noreferrer"
                   className={'link'}
                   download="Home_Saved Reports_GTOStatAnalyzer.report">Download the HM3 report.</a></li>
            <li>Locate the HM3 install directory. EG: "C:\Users\YOUR_NAME\AppData\Roaming\Max Value Software\Holdem
                Manager\3.0\"
            </li>
            <li>Paste the HM3 report into the saved reports folder: EG: C:\Users\YOUR_NAME\AppData\Roaming\Max Value
                Software\Holdem Manager\3.0\Reports\Saved\
            </li>
            <li>Open HM3.</li>
            <li>In the reports section, select the new GTOStatAnalyzer report.</li>
            <li>Right click the stats, "Select All".</li>
            <li>Right click the stats, "Save As".</li>
            <li>Use the button below to upload the saved file.</li>
        </ol>
    </Fragment>
}

export default Instructions;