import React from 'react';
import { node } from 'prop-types';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';

const ApplicationTemplate = ({ alertRenderer, children }) => {
  return (
    <Container maxWidth='xl' disableGutters>
      <Grid container direction="column" alignItems="stretch">
        {alertRenderer}
        {children}
      </Grid>
    </Container>
  )
}

ApplicationTemplate.propTypes = {
  children: node.isRequired,
}

export default ApplicationTemplate;
