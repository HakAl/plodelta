import {Fragment, useState} from "react";
import Papa from "papaparse";
import {
    GTO_MIDSTAKES_PREFLOP_VALUES,
    GTO_POSTFLOP_AS_TITLES,
    GTO_POSTFLOP_AS_VALUES,
    GTO_POSTFLOP_VS_TITLES,
    GTO_POSTFLOP_VS_VALUES,
    GTO_PREFLOP_KEYS, SNOWFLAKE_INDEX
} from "./appData";
import GTOTable from "./GTOTable";
import GTOSelect from "./GTOSelect";

function HM3Report() {
    const [preflopValues, setPreflopValues] = useState(null);
    const [postflopAsValues, setPostflopAsValues] = useState(null);
    const [postflopVsValues, setPostflopVsValues] = useState(null);
    const [preflopDeltas, setPreflopDeltas] = useState(null);
    const [postflopAsDeltas, setPostflopAsDeltas] = useState(null);
    const [postflopVsDeltas, setPostflopVsDeltas] = useState(null);
    const [preflopAverage, setpreflopAverage] = useState(null);
    const [postflopAsAverage, setPostflopAsAverage] = useState(null);
    const [postflopVsAverage, setPostflopVsAverage] = useState(null);

    //Callback to pass to Papaparse
    const complete = (results, file) => {
        const data = results.data;

        if (data && data.length > 1) {
            //clean values
            let values = data[1];
            //yuck
            let count = 0;
            values = values.map(value => {
                let result;
                if (count !== SNOWFLAKE_INDEX) {
                    result = (value * 100).toFixed(2);
                } else {
                    result = value;
                }
                count++;
                return result;
            })
            const preflopValues = values.slice(0, GTO_PREFLOP_KEYS.length);
            const postflopAsValues = values.slice(GTO_PREFLOP_KEYS.length, GTO_PREFLOP_KEYS.length +GTO_POSTFLOP_AS_TITLES.length);
            const postflopVsValues = values.slice((GTO_PREFLOP_KEYS.length +GTO_POSTFLOP_AS_TITLES.length));

            setPreflopValues(preflopValues);
            setPostflopAsValues(postflopAsValues);
            setPostflopVsValues(postflopVsValues);

            let preflopDeltas = [];
            let preflopSum = 0;
            for (let i = 0; i < GTO_MIDSTAKES_PREFLOP_VALUES.length; i++) {
                let result = Math.abs((preflopValues[i] - GTO_MIDSTAKES_PREFLOP_VALUES[i]));
                preflopDeltas.push(result.toFixed(2));
                preflopSum += result;
            }
            setpreflopAverage((preflopSum / GTO_MIDSTAKES_PREFLOP_VALUES.length).toFixed(2))
            setPreflopDeltas(preflopDeltas);

            let postflopAsDeltas = [];
            let postflopAsSum = 0;
            for (let i = 0; i < GTO_POSTFLOP_AS_VALUES.length; i++) {
                let result = Math.abs((postflopAsValues[i] - GTO_POSTFLOP_AS_VALUES[i]));
                postflopAsSum += result;
                postflopAsDeltas.push(result.toFixed(2));
            }
            setPostflopAsAverage((postflopAsSum / GTO_POSTFLOP_AS_VALUES.length).toFixed(2))
            setPostflopAsDeltas(postflopAsDeltas);

            let postflopVsDeltas = [];
            let postflopVsSum = 0;
            for (let i = 0; i < GTO_POSTFLOP_VS_VALUES.length; i++) {
                let result = Math.abs(postflopVsValues[i] - GTO_POSTFLOP_VS_VALUES[i]);
                postflopVsSum += result;
                postflopVsDeltas.push(result.toFixed(2));
            }
            setPostflopVsAverage((postflopVsSum / GTO_POSTFLOP_VS_VALUES.length).toFixed(2));
            setPostflopVsDeltas(postflopVsDeltas);
        }
    }
    const onPreflopInputChange = (evt) => {
        if (evt && evt.target.files && evt.target.files.length) {
            Papa.parse(evt.target.files[0], {complete});
        }
    }
    const reportInputProps = {
        name: 'reportInput',
        type: "file",
        onChange: onPreflopInputChange,
    };
    const preflopTableProps = {
        playerValues: preflopValues,
        playerDeltas: preflopDeltas,
        average: preflopAverage,
        title: "Preflop",
        gtoTitles: GTO_PREFLOP_KEYS,
        gtoValues: GTO_MIDSTAKES_PREFLOP_VALUES,
    }
    const postflopAsTableProps = {
        playerValues: postflopAsValues,
        playerDeltas: postflopAsDeltas,
        average: postflopAsAverage,
        title: "Postflop as Aggressor",
        gtoTitles: GTO_POSTFLOP_AS_TITLES,
        gtoValues: GTO_POSTFLOP_AS_VALUES,
    }
    const postflopVsTableProps = {
        playerValues: postflopVsValues,
        playerDeltas: postflopVsDeltas,
        average: postflopVsAverage,
        title: "Postflop VS Aggressor",
        gtoTitles: GTO_POSTFLOP_VS_TITLES,
        gtoValues: GTO_POSTFLOP_VS_VALUES,
    }

    return (
        <Fragment>
            <div className={'row'}>
                <div className={'input_container'}>
                    <input {...reportInputProps}/>
                </div>
                <div className={'column'}>
                    <GTOTable {...preflopTableProps} />
                </div>
                <div className={'column'}>
                    <GTOTable {...postflopAsTableProps} />
                </div>
                <div className={'column'}>
                    <GTOTable {...postflopVsTableProps} />
                </div>
            </div>
        </Fragment>
    );
}

export default HM3Report;
