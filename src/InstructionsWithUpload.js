// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import {useState} from "react";
import Papa from "papaparse";

function InstructionsWithUpload({complete}) {
    const [fileName, setFileName] = useState('No file selected.');
    const [isCollapsed, setIsCollapsed] = useState(false);

    const onPreflopInputChange = (evt) => {
        if (evt && evt.target.files && evt.target.files.length) {
            const file = evt.target.files[0];
            setFileName(file.name);
            Papa.parse(file, {complete});
            setIsCollapsed(true);
        } else {
            setFileName('No file selected.');
            setIsCollapsed(false);
        }
    }

    const toggleCollapse = () => {
        setIsCollapsed(!isCollapsed);
    }

    const reportInputProps = {
        id: 'file-upload',
        name: 'reportInput',
        type: "file",
        accept: ".csv",
        onChange: onPreflopInputChange,
    };

    const hasFile = fileName !== 'No file selected.';

    if (isCollapsed) {
        return (
            <div className="body-instructions collapsed">
                <div className="collapsed-header" onClick={toggleCollapse}>
                    <div className="collapsed-info">
                        <svg
                            className="file-icon"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                        >
                            <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth="2"
                                d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                            />
                        </svg>
                        <span className="file-name">{fileName}</span>
                    </div>
                    <svg
                        className="expand-icon"
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                    >
                        <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            strokeWidth="2"
                            d="M19 9l-7 7-7-7"
                        />
                    </svg>
                </div>
            </div>
        );
    }

    return (
        <div className="body-instructions">
            <div className="instructions-header">
                <h2 className="mb-4">
                    Analyze your poker stats with{' '}
                    <a
                        href="https://www.holdemmanager.com/hm3/download.php"
                        rel="noreferrer"
                        className="link"
                        target="_blank"
                    >
                        Holdem Manager 3
                    </a>
                </h2>
                {hasFile && (
                    <button
                        className="collapse-button"
                        onClick={toggleCollapse}
                        title="Collapse instructions"
                    >
                        <svg
                            className="collapse-icon"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                        >
                            <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth="2"
                                d="M5 15l7-7 7 7"
                            />
                        </svg>
                    </button>
                )}
            </div>

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
                    </a>{' '}
                    (Windows only)
                </li>
                <li>Open Holdem Manager 3</li>
                <li>Go to the Reports section and select the GTOStatAnalyzer report</li>
                <li>Right-click the stats table and choose "Select All"</li>
                <li>Right-click again and choose "Save As" to export your data</li>
                <li>Upload the saved file using the button below</li>
            </ol>

            <div className="instructions-divider"></div>

            <div className='upload-card'>
                <div className="upload-header">
                    <svg
                        className="upload-icon"
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg"
                    >
                        <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            strokeWidth="2"
                            d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
                        />
                    </svg>
                    <h3>Upload Your Report</h3>
                    <p className="upload-description">
                        Ready to analyze your stats? Upload the CSV file you saved from HM3.
                    </p>
                </div>
                <div className={'input_container'}>
                    <label htmlFor="file-upload" className="file-upload-label">
                        <svg
                            className="button-icon"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                        >
                            <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth="2"
                                d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                            />
                        </svg>
                        Choose File
                    </label>
                    <input {...reportInputProps} />
                    <div className={`file-status ${hasFile ? 'file-selected' : ''}`}>
                        {hasFile && (
                            <svg
                                className="check-icon"
                                fill="none"
                                stroke="currentColor"
                                viewBox="0 0 24 24"
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    strokeWidth="2"
                                    d="M5 13l4 4L19 7"
                                />
                            </svg>
                        )}
                        {fileName}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default InstructionsWithUpload;
