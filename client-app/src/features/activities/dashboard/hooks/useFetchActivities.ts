import { useEffect } from 'react';
import { useMobXStore } from '@store/index';

export default function useFetchActivities(status: 0 | 1 | 2 = 1) {
  const {
    activityStore: { fetchActivities },
  } = useMobXStore();

  useEffect(() => {
    fetchActivities(true, status).then();
  }, [fetchActivities, status]);
}
