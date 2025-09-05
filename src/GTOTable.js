import {Fragment} from "react";
import GTOColumn from "./GTOColumn";

function GTOTable({preflopTableProps, postflopAsTableProps, postflopVsTableProps}) {
    return <Fragment>
        <div className={'column'}>
            <GTOColumn {...preflopTableProps} />
        </div>
        <div className={'column'}>
            <GTOColumn {...postflopAsTableProps} />
        </div>
        <div className={'column'}>
            <GTOColumn {...postflopVsTableProps} />
        </div>
    </Fragment>
}

export default GTOTable;