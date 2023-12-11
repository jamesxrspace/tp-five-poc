describe('template spec', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000/');
  });

  it('will redirect to space list on click logo', () => {
    cy.get('a').findByText('One CMS').click();
    cy.location('pathname').should('eq', '/space/list');
  });
});
