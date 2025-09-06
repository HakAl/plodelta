// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import {Fragment, useState} from "react";

const isTenPercentOff = (gto, delta) => {
    return Math.abs(((delta / gto) * 100)).toFixed() >= 10;
}

function GTOColumn({title, gtoTitles, gtoValues, playerValues, playerDeltas, average}) {
    const [editIndex, setEditIndex] = useState(undefined);
    const [userEdits, setUserEdits] = useState([]);

    const hasPlayerValues = playerValues && playerValues.length > 0;
    const onGTOValueEdit = (edit) => {

        userEdits[edit.index] = edit.value;
        setUserEdits(userEdits);
    };
    const onGTOValueSave = (i) => {
        const toValidate = userEdits[i];
        const isNumberRegex = /^\d+$/;
        if (!isNumberRegex.test(toValidate)) {
            userEdits[i] = undefined;
            userEdits.splice(i, 1);
            setUserEdits(userEdits);
        }
        setEditIndex(undefined);
    }
    const onGTOValueClose = (i) => {
        userEdits[i] = undefined;
        userEdits.splice(i, 1);
        setEditIndex(undefined);
    }

    return (
        <Fragment>
            <table className="styled-table">
                <thead>
                <tr>
                    <th>{title}</th>
                    <th>GTO</th>
                    <th>YOU</th>
                    <th>&#916;</th>
                </tr>
                </thead>
                <tbody>
                {gtoTitles.map((title, i) => {
                        let playerValueClassName = '';
                        if (hasPlayerValues) {
                            playerValueClassName = isTenPercentOff(gtoValues[i], (playerValues[i] - gtoValues[i]).toFixed(2))
                                ? 'bad' : 'good';
                        }
                        const hasUserEdit = typeof userEdits[i] !== 'undefined' && userEdits[i] !== undefined;
                        const updatedValue = hasUserEdit ? userEdits[i] : gtoValues[i];
                        let updatedDelta;
                        if (playerDeltas) {
                            updatedDelta = hasUserEdit ? Math.abs(userEdits[i] - playerValues[i]).toFixed(2) : playerDeltas[i]
                        }

                        return <tr key={i}>
                            <td><b>{title}</b></td>
                            {editIndex === i
                                ? <td>
                                    <input onChange={(event) => {
                                        if (event && event.target && event.target.value) {
                                            onGTOValueEdit({index: i, value: event.target.value});
                                        }
                                    }}/>
                                    <i onClick={() => onGTOValueSave(i)}
                                       style={{'paddingLeft': '4px', 'paddingRight': '4px'}}
                                       className={'glyphicon glyphicon-ok'}/>
                                    <i onClick={() => onGTOValueClose(i)}
                                       style={{'paddingLeft': '4px', 'paddingRight': '4px'}}
                                       className={'glyphicon glyphicon-remove'}/></td>
                                : <td>{updatedValue}
                                    <i onClick={() => setEditIndex(i)}
                                       style={{'paddingLeft': '16px', 'paddingRight': '4px'}}
                                       className={'glyphicon glyphicon-pencil'}/>
                                </td>
                            }
                            <td>{hasPlayerValues ? playerValues[i] : '-'}</td>
                            <td className={playerValueClassName}>
                                {playerDeltas ? updatedDelta : '-'}
                            </td>
                        </tr>;
                    }
                )}
                </tbody>
                <tfoot>
                <tr>
                    <td>Average Deviation</td>
                    <td></td>
                    <td></td>
                    <td>{average}</td>
                </tr>
                </tfoot>
            </table>
        </Fragment>
    );
}

export default GTOColumn;
