import * as React from "react";
import _ from "underscore";
import lodash from "lodash";

interface Props {
  tags: string[][];
}

interface CheckedTag {
  tag: string;
  checked: boolean;
}

interface State {
  tags: CheckedTag[]
}

class Criteria extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.setState({
      tags: this.tags().map(t => {
        return { tag: t, checked: false };
      })
    });
  }

  tags(): string[] {
    let results = this.props.tags.flatMap(tag => tag.flatMap(t => t));
    results = _.uniq(results);
    results = _.sortBy(results);

    return results;
  }

  render() {
    return (
      <div className="criteria">
        {
          this.tags().map(tag => 
          <div className="checkbox">
            <input id={`${tag}`} type="checkbox" name={tag} />
            <label htmlFor={`${tag}`}>{lodash.startCase(tag)}</label>
          </div>
          )
        }
      </div>
    )
  }
}

export { Criteria };
