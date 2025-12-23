export const AppMessages = {
    UnexpectedError: 'An unexpected error has occurred. Please try again later.',
    DataSavedSuccessfully: 'Your data has been saved successfully.',
    DataLoadError: 'Failed to load data. Please refresh the page.',
    UnauthorizedAccess: 'You are not authorized to perform this action.',
    InvalidInput: 'The input provided is invalid. Please check and try again.',
    OperationInProgress: 'Operation in progress. Please wait...',
    NoDataAvailable: 'No data available to display.',
    SessionExpired: 'Your session has expired. Please log in again.',
    ActionCompleted: 'The action has been completed successfully.',
    NetworkError: 'A network error occurred. Please check your connection.'
  } as const;
  
  export type AppMessageKey = keyof typeof AppMessages;