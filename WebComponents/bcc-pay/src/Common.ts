export function isNullOrEmpty(objectToCheck: any): boolean {
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
