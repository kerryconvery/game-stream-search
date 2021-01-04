export const autoMockObject  = (object) => {
  const mock = {};

  Object.keys(object).forEach(key => {
    mock[key] = jest.fn();
  });

  return mock;
}