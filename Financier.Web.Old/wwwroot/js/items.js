const apolloClient = new Apollo.lib.ApolloClient({
    networkInterface: Apollo.lib.createNetworkInterface({
        uri: 'http://localhost:5000/graphql'
    })
});

document.renderItems = function(parent) {
    const query = Apollo.gql`
        {
          itemsByTagId(
            tagIds: [
              "bb111f65-e2d4-425b-be33-4d0db6c2619b",
              "3129045d-1de9-4a8f-9fd5-8944c4e9ff1a"
            ]
          ) {
            id
            amount
            description
          }
        }
    `;

    apolloClient
        .query({
            query: query
        }).then(result => {
            result.data.itemsByTagId.forEach(item => {
                parent.innerHTML += `
                    <div>${item.description}: ${item.amount}</div>
                `;
            });
        })
};
