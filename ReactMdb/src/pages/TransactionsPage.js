import React, { Component } from 'react';
import { MDBContainer, MDBRow, MDBCol, MDBCard, MDBCardBody, MDBDataTableV5 } from 'mdbreact';
import SectionContainer from '../components/sectionContainer';

class TransactionsPage extends Component {
  state = {
    isLoading: true,
    data: {
      columns: [],
      rows: [],
    },
  };

  componentDidMount() {
    fetch("http://localhost:5000/transactions")
      .then((response) => response.json())
      .then((data) => {
        const newData = 
        {
          columns: [
            {
              label: 'Data',
              field: 'date',
              width: 150,
              attributes: {
                'aria-controls': 'DataTable',
                'aria-label': 'Name',
              },
            },
            {
              label: 'Konto',
              field: 'account',
              width: 270,
            },
            {
              label: 'Kategoria',
              field: 'category',
              width: 200,
            },
            {
              label: 'Kwota',
              field: 'amount',
              sort: 'asc',
              width: 100,
            },
            {
              label: 'Szczegóły',
              field: 'detale',
              width: 150,
            },
            {
              label: 'Osoba',
              field: 'person',
              width: 100,
            },
          ],
          rows: data
        }
        this.setState( 
          { 
            data: newData, 
            isLoading: false 
          })
        })
  }
  

  render() {
    const { data, isLoading } = this.state;

    
    return (
      <MDBContainer className='mt-3'>
        <div>
              {isLoading ? (
                 "Loading"
               ) : (
                <MDBRow className='py-3'>
          <MDBCol md='12'>
            <SectionContainer title='Tranzakcje' noBorder>
              <MDBCard>
                <MDBCardBody>
                  <MDBDataTableV5
                    hover
                    data={data}
                    searchTop
                    searchBottom={false}
                    paging={false}
                    displayEntries={false}
                    sortRows={['badge']}
                    order={['age', 'desc']}
                  />
                </MDBCardBody>
              </MDBCard>
            </SectionContainer>
          </MDBCol>
        </MDBRow>
               )}
         </div>
       
      </MDBContainer>
    );
  }
}

export default TransactionsPage;
