export function isNullOrEmpty(objectToCheck): boolean {
  if (
    objectToCheck === null ||
    objectToCheck === '' ||
    objectToCheck === undefined
  ) {
    return true;
  } else {
    return false;
  }
}
