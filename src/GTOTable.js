// GTOTable.js - Updated with tabs
import { useState } from "react";
import GTOColumn from "./GTOColumn";

function GTOTable({ preflopTableProps, postflopAsTableProps, postflopVsTableProps }) {
    const [activeTab, setActiveTab] = useState('preflop');

    return (
        <div className="gto-table-container">
            <ul className="nav nav-tabs gto-tabs">
                <li className="nav-item">
                    <button
                        className={`nav-link ${activeTab === 'preflop' ? 'active' : ''}`}
                        onClick={() => setActiveTab('preflop')}
                    >
                        Preflop
                    </button>
                </li>
                <li className="nav-item">
                    <button
                        className={`nav-link ${activeTab === 'postflopAs' ? 'active' : ''}`}
                        onClick={() => setActiveTab('postflopAs')}
                    >
                        Postflop as Aggressor
                    </button>
                </li>
                <li className="nav-item">
                    <button
                        className={`nav-link ${activeTab === 'postflopVs' ? 'active' : ''}`}
                        onClick={() => setActiveTab('postflopVs')}
                    >
                        Postflop VS Aggressor
                    </button>
                </li>
            </ul>

            <div className="tab-content">
                {activeTab === 'preflop' && (
                    <div className="tab-pane active">
                        <GTOColumn {...preflopTableProps} />
                    </div>
                )}
                {activeTab === 'postflopAs' && (
                    <div className="tab-pane active">
                        <GTOColumn {...postflopAsTableProps} />
                    </div>
                )}
                {activeTab === 'postflopVs' && (
                    <div className="tab-pane active">
                        <GTOColumn {...postflopVsTableProps} />
                    </div>
                )}
            </div>
        </div>
    );
}

export default GTOTable;