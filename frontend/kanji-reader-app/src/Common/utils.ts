export const getRatioColor = (ratio: number) => {
  if (ratio <= 0.1) return "success";
  if (ratio <= 0.3) return "warning";
  return "error";
};

export const getRatioLabel = (ratio: number) => {
  if (ratio <= 0.1) return "Easy";
  if (ratio <= 0.3) return "Medium";
  return "Hard";
};

export const formatDate = (utcDateString: string): string => {
  try {
    const date = new Date(utcDateString);
    
    if (isNaN(date.getTime())) {
      return 'Invalid date';
    }

    const userTimezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    
    const formatter = new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      timeZone: userTimezone,
      timeZoneName: 'short'
    });

    return formatter.format(date);
  } catch (error) {
    console.error('Error formatting date:', error);
    return 'Invalid date';
  }
};

export const getRelativeTime = (utcDateString: string): string => {
  try {
    const date = new Date(utcDateString);
    if (isNaN(date.getTime())) {
      return 'Invalid date';
    }

    const now = new Date();
    const diffInMs = now.getTime() - date.getTime();
    const diffInMinutes = Math.floor(diffInMs / (1000 * 60));
    const diffInHours = Math.floor(diffInMs / (1000 * 60 * 60));
    const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

    if (diffInMinutes < 1) {
      return 'Just now';
    } else if (diffInMinutes < 60) {
      return `${diffInMinutes} minute${diffInMinutes > 1 ? 's' : ''} ago`;
    } else if (diffInHours < 24) {
      return `${diffInHours} hour${diffInHours > 1 ? 's' : ''} ago`;
    } else if (diffInDays < 7) {
      return `${diffInDays} day${diffInDays > 1 ? 's' : ''} ago`;
    } else {
      return formatDate(utcDateString);
    }
  } catch (error) {
    return 'Invalid date';
  }
};


export const getErrorMessage = (error: any): string => {
  if (error?.response?.data?.message) {
    return error.response.data.message;
  }
  if (error?.response?.data?.error) {
    return error.response.data.error;
  }
  if (error?.message) {
    return error.message;
  }
  return "An unexpected error occurred. Please try again later.";
};
