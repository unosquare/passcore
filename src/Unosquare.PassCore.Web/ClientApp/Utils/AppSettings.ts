export async function resolveAppSettings(): Promise<any> {
    const response = await fetch('api/password');

    if (!response || response.status !== 200) {
        throw new Error('Error fetching settings.');
    }

    const responseBody = await response.text();

    const data = responseBody ? JSON.parse(responseBody) : {};

    return { ...data };
}
