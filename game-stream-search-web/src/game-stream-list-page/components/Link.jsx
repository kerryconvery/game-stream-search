import { styled } from '@material-ui/core/styles';

const Link = styled('a')({
  textDecoration: 'none',
  color: 'inherit',
  '&:hover > button': {
    opacity: 0.7
  },
})

export default Link;