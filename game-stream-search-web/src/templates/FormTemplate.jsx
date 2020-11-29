import React from 'react';
import { string, node, func, bool } from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardHeader from '@material-ui/core/CardHeader';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Button from '@material-ui/core/Button';
import CircularProgress from '@material-ui/core/CircularProgress';

const useFormTemplateStyles = makeStyles({
  card: {
    '& *': {
      fontFamily: 'Helvetica',
    }
  },
  title: {
    paddingBottom: '0.5rem',
  },
  formContent: {
    paddingTop: 0,
  },
});

const FormTemplate = ({ title, content, children }) => {
  const classes = useFormTemplateStyles();

  return (
    <Card className={classes.card}>
      <CardHeader className={classes.title} title={title} />
      <CardContent className={classes.formContent}>
        {content}
      </CardContent>
      <CardActions>
        {children}
      </CardActions>
    </Card>
  )
}

FormTemplate.propTypes = {
  title: string.isRequired,
  content: node.isRequired,
  children: node.isRequired,
}

const useSubmitButtonStyles = makeStyles({
  root: {
    width: '4rem',
  },
  center: {
    display: 'block',
    margin: 'auto'
  }
});

export const SubmitButton = ({ children, onClick, submitting }) => {
  const classes = useSubmitButtonStyles();

  return (
    <div className={classes.root}>
      {submitting ? <CircularProgress className={classes.center} size={30} /> : <Button onClick={onClick}>{children}</Button>}
    </div>
  )
}

SubmitButton.propTypes = {
  children: string.isRequired,
  onClick: func.isRequired,
  submitting: bool,
}

SubmitButton.defaultProps = {
  submitting: false,
}

export default FormTemplate;